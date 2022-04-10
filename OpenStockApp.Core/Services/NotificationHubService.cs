using OpenStockApi.Core.Models.Searches;
using OpenStockApp.Core.Contracts.Services;
using OpenStockApp.Core.Contracts.Services.Hubs;
using OpenStockApp.Core.Contracts.Services.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenStockApp.Core.Services
{
    public class NotificationHubService : INotificationHubService
    {
        public event EventHandler<Result>? NotificationReceived;
        private readonly IEnumerable<INotificationService> notificationServices;
        private readonly IFilterService filterService;

        public NotificationHubService(IEnumerable<INotificationService> notificationService, IFilterService filterService)
        {
            this.notificationServices = notificationService;
            this.filterService = filterService;
        }

        public async Task ForwardNotificationReceived(Result? result, CancellationToken cancellationToken = default)
        {
            if (result == null)
                return;
            if (filterService.CanShowNotification(result))
            {
                var tasks = new List<Task>();
                foreach (var notificationService in notificationServices)
                {
                    tasks.Add(notificationService.SendNotificationAsync(result));
                }
                await Task.WhenAll(tasks);                
            }
            //the event must be sent regardless.
            OnNotificationReceived(result);
        }
        public async Task ForwardNotificationReceived(IEnumerable<Result> results, CancellationToken cancellationToken = default)
        {
            var tasks = new List<Task>();
            foreach(var result in results)
            {
                tasks.Add(ForwardNotificationReceived(result, cancellationToken));
            }
            await Task.WhenAll(tasks);
        }
        protected void OnNotificationReceived(IEnumerable<Result> results)
        {
            foreach(var result in results)
            {
                OnNotificationReceived(result);
            }
        }
        protected void OnNotificationReceived(Result result)
        {
            NotificationReceived?.Invoke(this, result);
        }

    }
}
