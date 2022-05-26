using Microsoft.Extensions.Caching.Memory;
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
        public IMemoryCache memoryCache;

        public NotificationHubService(IEnumerable<INotificationService> notificationService,
                                      IFilterService filterService,
                                      IMemoryCache memoryCache)
        {
            this.notificationServices = notificationService;
            this.filterService = filterService;
            this.memoryCache = memoryCache;
        }

        public async Task ForwardNotificationReceived(Result? result, bool doNotFilter = false, CancellationToken cancellationToken = default)
        {
            if (result == null)
                return;
            if (memoryCache.TryGetValue(result.Id, out bool _))
            {
                return;
            }
            else
            {
                memoryCache.Set(result.Id, true, TimeSpan.FromMinutes(5));
            }

            if (doNotFilter || filterService.CanShowNotification(result))
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
                tasks.Add(ForwardNotificationReceived(result, cancellationToken: cancellationToken));
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
