using OpenStockApi.Core.Models.Searches;
using OpenStockApp.Core.Contracts.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserNotifications;

namespace OpenStockApp.Core.Maui.Platforms.MacCatalyst.Notifications
{
    public class MacOsNotificationService : INotificationService
    {
        public async Task SendNotificationAsync(Result result)
        {
            var builder = new MacOsNotificaitonBuilder();
            var notification = builder.WithResult(result).Build();
            var request = UNNotificationRequest.FromIdentifier(
                    Guid.NewGuid().ToString(),
                    notification,
                    null
                );
            await UNUserNotificationCenter.Current.AddNotificationRequestAsync(request);
        }

        public async Task SendNotificationAsync(IEnumerable<Result> results)
        {
            foreach(var result in results)
            {
                await SendNotificationAsync(result);
            }
        }
    }
}
