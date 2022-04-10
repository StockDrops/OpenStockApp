using OpenStockApi.Core.Models.Searches;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenStockApp.Core.Contracts.Services
{
    public interface INotificationHubService
    {
        public event EventHandler<Result>? NotificationReceived;
        public Task ForwardNotificationReceived(Result? result, CancellationToken cancellationToken = default);
        public Task ForwardNotificationReceived(IEnumerable<Result> results, CancellationToken cancellationToken = default);
    }
}
