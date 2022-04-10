using OpenStockApi.Core.Models.Searches;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenStockApp.Core.Contracts.Services
{
    /// <summary>
    /// An interface definining a notification service to be used in multiple platforms.
    /// </summary>
    public interface INotificationService
    {
        public Task SendNotificationAsync(Result result);
        public Task SendNotificationAsync(IEnumerable<Result> results);
    }
}
