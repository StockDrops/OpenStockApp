using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Toolkit.Diagnostics;
using OpenStockApi.Core.Contracts.Hubs;
using OpenStockApi.Core.Models.Searches;
using OpenStockApi.Core.Models.Users;
using OpenStockApp.Core.Contracts.Services;
using OpenStockApp.Core.Contracts.Services.Hubs;
using OpenStockApp.Core.Contracts.Services.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenStockApp.SignalR.Services
{
    public class NotificationsHubClientConfiguration : BaseHubConfiguration
    {

    }
    public class NotificationsHubClient : BaseHubClient,  IHostedService, INotificationsHubClient
    {
        private readonly INotificationHubService notificationHubService;
        public NotificationsHubClient(INotificationHubService notificationHubService,
            IIdentityService identityService, 
            IOptions<NotificationsHubClientConfiguration> options, 
            ILogger<BaseHubClient> logger) : base(identityService, options, logger)
        {
            this.notificationHubService = notificationHubService;
        }

        public IAsyncEnumerable<Result> GetLatestResults(int pageSize, int pageNumber, CancellationToken cancellationToken = default)
        {
            var options = new ResultOptions { PageSize = pageSize, PageNumber = pageNumber };
            try
            {
                return connection.StreamAsync<Result>("GetLatestResults", options, cancellationToken);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to load latest results");
            }
            return AsyncEnumerable.Empty<Result>();
        }

        public IAsyncEnumerable<Result> GetFilteredResults(UserOptions? userOptions, int pageSize, int pageNumber, CancellationToken cancellationToken = default)
        {
            Guard.IsNotNull(userOptions, nameof(userOptions));
            var options = new ResultOptions { PageSize = pageSize, PageNumber = pageNumber };
            try
            {
                return connection.StreamAsync<Result>("GetFilteredResults", userOptions.Id, options, cancellationToken);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to load latest results");
            }
            return AsyncEnumerable.Empty<Result>();
        }

        protected override void RegisterHubFunctions()
        {
            connection.On<IEnumerable<Result>>(nameof(IClientNotificationsHub.SendMultipleNotification), async (results) => 
            {
                await notificationHubService.ForwardNotificationReceived(results);                
            });
            connection.On<Result?>(nameof(IClientNotificationsHub.SendSingleNotification), async (result) => {
                await notificationHubService.ForwardNotificationReceived(result);
                
            });
        }

        public async Task SendTestNotification(CancellationToken cancellationToken = default)
        {
            try
            {
                await connection.SendAsync("SendTestNotification", cancellationToken);
            }
            catch(Exception ex)
            {
                logger.LogError(ex, "");
            }
        }
    }
}
