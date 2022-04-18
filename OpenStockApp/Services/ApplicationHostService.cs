using Microsoft.Extensions.Logging;
using OpenStockApp.Core.Contracts.Services;
using OpenStockApp.Core.Contracts.Services.Hubs;
using OpenStockApp.Core.Contracts.Services.Settings;
using OpenStockApp.Core.Contracts.Services.Users;
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
        /// <summary>
        /// Default constructor with all the dependencies.
        /// </summary>
        /// <param name="identityService"></param>
        public ApplicationHostService(IIdentityService identityService,
            IUserOptionsService userOptionsService, 
            ILogger<ApplicationHostService> logger,
            IEnumerable<IBaseHubClient> hubClients,
            INotificationHubService notificationHubService
            )
        {
            this.identityService = identityService;
            this.hubClients = hubClients;
            this.userOptionsService = userOptionsService;
            this.notificationHubService = notificationHubService;
            this.logger = logger;
#if ANDROID
            MessagingCenter.Subscribe<FirebaseService, Result>(this, "NotificationReceived", async (sender, args) =>
            {
                await OnNotificationReceived(sender, args);
            });
#endif
        }

#if ANDROID
        private async Task OnNotificationReceived(FirebaseService sender, Result remoteMessage)
        {
            await notificationHubService.ForwardNotificationReceived(remoteMessage);
        }
#endif

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
                    tasks.Add(hubClient.StartAsync(cancellationToken));
                }
                await Task.WhenAll(tasks); //about 3 s
                await userOptionsService.InitializeAsync(cancellationToken); //hubs need to be initialized first.
            }
            catch(Exception ex)
            {
                logger.LogError(ex, "Error while initializing the app.");
            }
            
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
