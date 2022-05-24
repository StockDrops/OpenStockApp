using Microsoft.Extensions.Logging;
using OpenStockApp.Core.Contracts.Services;
using OpenStockApp.Core.Contracts.Services.Hubs;
using OpenStockApp.Core.Contracts.Services.Settings;
using OpenStockApp.Core.Contracts.Services.Users;
using OpenStockApp.Core.Models.Events;
#if ANDROID
using OpenStockApp.Platforms.Android;
#endif
using OpenStockApp.SignalR.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Result = OpenStockApi.Core.Models.Searches.Result;

namespace OpenStockApp.Services
{
    /// <summary>
    /// Handles lifetimes of objects in it.
    /// </summary>
    public class ApplicationHostService
    {
        private readonly IIdentityService identityService;
        private readonly IEnumerable<IBaseHubClient> hubClients;
        private readonly IUserOptionsService userOptionsService;
        private readonly ILogger<ApplicationHostService> logger;
        private readonly INotificationHubService notificationHubService;
        private readonly ITokenRegistrationService tokenRegistrationService;
        /// <summary>
        /// Default constructor with all the dependencies.
        /// </summary>
        /// <param name="identityService"></param>
        public ApplicationHostService(IIdentityService identityService,
            IUserOptionsService userOptionsService, 
            ILogger<ApplicationHostService> logger,
            IEnumerable<IBaseHubClient> hubClients,
            INotificationHubService notificationHubService,
            ITokenRegistrationService tokenRegistrationService
            )
        {
            this.identityService = identityService;
            this.hubClients = hubClients;
            this.userOptionsService = userOptionsService;
            this.notificationHubService = notificationHubService;
            this.tokenRegistrationService = tokenRegistrationService;
          
            this.logger = logger;
            AppDomain.CurrentDomain.FirstChanceException += OnFirstChanceException;
#if ANDROID
            MessagingCenter.Subscribe<FirebaseService, Result>(this, "NotificationReceived", async (sender, args) =>
            {
                await OnNotificationReceived(sender, args);
            });
#endif
#if IOS
            MessagingCenter.Subscribe<AppDelegate, string>(this, Events.RegisterToken, async (sender, args) =>
            {
                var tokenSource = new CancellationTokenSource();
                tokenSource.CancelAfter(10000);
                await tokenRegistrationService.RegisterTokenAsync(args, tokenSource.Token);
            });
#endif
        }

#if ANDROID
        private async Task OnNotificationReceived(FirebaseService sender, Result remoteMessage)
        {
            await notificationHubService.ForwardNotificationReceived(remoteMessage);
        }
#endif
        private void OnFirstChanceException(object? sender, System.Runtime.ExceptionServices.FirstChanceExceptionEventArgs e)
        {
            logger.LogWarning(e.Exception, "Unhandled exception");
        }

        /// <summary>
        /// Starts the services as required.
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task StartAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                var tasks = new List<Task>();
                await identityService.InitializeAsync(cancellationToken); //about 3.2 s

                foreach (var hubClient in hubClients)
                {
                    tasks.Add(StartHub(hubClient, cancellationToken));
                }
                await Task.WhenAll(tasks); //about 3 s
                await userOptionsService.InitializeAsync(cancellationToken); //hubs need to be initialized first.
            }
            catch(Exception ex)
            {
                logger.LogError(ex, "Error while initializing the app.");
            }
            
        }
        private async Task StartHub(IBaseHubClient hubClient, CancellationToken cancellationToken = default)
        {
            if (hubClient.State != Microsoft.AspNetCore.SignalR.Client.HubConnectionState.Connected)
                await hubClient.StartAsync(cancellationToken).ConfigureAwait(false);
        }
        public async Task StopAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                foreach(var hubClient in hubClients)
                {
                   await hubClient.StopAsync(cancellationToken);
                }
            }
            catch(Exception ex)
            {
                logger.LogError(ex, "Error when closing/stopping the app.");
            }
        }
    }
}
