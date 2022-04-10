using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OpenStockApp.Core.Contracts.Services.Hubs;
using OpenStockApp.Core.Contracts.Services.Users;
using Polly;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenStockApp.SignalR.Services
{
    public class BaseHubConfiguration
    {
        public string? HubUrl { get; set; }
    }
    public abstract class BaseHubClient : IBaseHubClient
    {
        /// <summary>
        /// The hub connection of the client
        /// </summary>
        protected readonly HubConnection connection;

        protected readonly IIdentityService identityService;
        protected readonly ILogger<BaseHubClient> logger;
        private readonly IOptions<BaseHubConfiguration> options;

        public event EventHandler<Exception?>? Closed;
        public event EventHandler? Reconnecting;
        public event EventHandler? Reconnected;
        public event EventHandler? Connected;
        public HubConnectionState State => connection.State;
        public string? ConnectionId => connection?.ConnectionId;
        public BaseHubClient(IIdentityService identityService,
            IOptions<BaseHubConfiguration> options,
            ILogger<BaseHubClient> logger)
        {
            this.identityService = identityService;
            this.logger = logger;
            this.options = options;

            RegisterEvents();

            if (string.IsNullOrEmpty(options?.Value?.HubUrl))
                throw new ArgumentNullException("The HubUrl must not be null!");

            connection = new HubConnectionBuilder()
                .WithUrl(options.Value.HubUrl, options =>
                {
                    options.AccessTokenProvider = async () =>
                    {
                        var time = new Stopwatch();
                        time.Start();
                        var token = await identityService.GetAccessTokenAsync();
                        time.Stop();

                        return token;
                    };
                })
                .WithAutomaticReconnect()
                .Build();
            RegisterHubEventsFunctions();
        }
        protected virtual void RegisterEvents()
        {
            identityService.LoggedIn += OnLoggedIn;
        }
        /// <summary>
        /// Actions to do in case of logged in user.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="eventArgs"></param>
        protected virtual async void OnLoggedIn(object? sender, EventArgs eventArgs)
        {
            await StartConnection();
        }
        /// <summary>
        /// It starts the connection if the user is logged in.
        /// </summary>
        /// <returns></returns>
        protected virtual async Task StartConnection(CancellationToken cancellationToken = default)
        {
            if (identityService.IsLoggedIn())
            {
                try
                {
                    if(connection.State != HubConnectionState.Connected)
                    {
                        await connection.StartAsync(cancellationToken);
                        RegisterHubFunctions();
                        Connected?.Invoke(this, EventArgs.Empty);
                    }
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "");
                }
            }
            
        }
        protected abstract void RegisterHubFunctions();
        protected virtual void RegisterHubEventsFunctions()
        {
            connection.Closed += OnClosed;
            connection.Reconnected += OnReconnected;
            
        }
        protected virtual Task OnReconnected(string? connectionId)
        {
            Reconnected?.Invoke(this, EventArgs.Empty);
            return Task.CompletedTask;
        }
        

        protected virtual async Task OnClosed(Exception? exception)
        {
            Closed?.Invoke(this, exception);
            if (exception != null)
            {
                //the server closed the connection, reconnect with retry logic.
                var rnd = new Random();
                await Policy
                    .Handle<Exception>()
                    .WaitAndRetryForeverAsync((tries) => TimeSpan.FromSeconds((Math.Pow(2, tries) + 1)*(rnd.NextDouble()+0.5))) //the ammount of time to wait is modulated around the exponential back off. This ensure not all clients will retry at exactly the same moment, DDoSing the server.
                    .ExecuteAsync(async () => {
                        if (connection.State == HubConnectionState.Disconnected)
                        {
                            await connection.StartAsync();
                            Connected?.Invoke(this, EventArgs.Empty);
                        }});
            }
        }

        public virtual Task StartAsync(CancellationToken cancellationToken = default)
        {
            return StartConnection(cancellationToken);
        }

        public virtual Task StopAsync(CancellationToken cancellationToken = default)
        {
            return connection.StopAsync(cancellationToken);
        }
    }
}
