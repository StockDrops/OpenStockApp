using OpenStockApp.Core.Contracts.Services;
using OpenStockApi.Core.Models.Searches;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserNotifications;

namespace OpenStockApp.Services.Notifications
{
    public class NotificationService : INotificationService
    {
        public async Task SendNotificationAsync(Result result)
        {
            try
            {
                var builder = new NotificationBuilder();
                var notification = builder.WithResult(result).Build();
                var request = UNNotificationRequest.FromIdentifier(
                        Guid.NewGuid().ToString(),
                        notification,
                        null
                    );
                await UNUserNotificationCenter.Current.AddNotificationRequestAsync(request);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
            }

        }

        public async Task SendNotificationAsync(IEnumerable<Result> results)
        {
            foreach (var result in results)
            {
                await SendNotificationAsync(result);
            }
        }
    }
}
