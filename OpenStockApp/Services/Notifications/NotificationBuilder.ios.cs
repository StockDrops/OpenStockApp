using Foundation;
using OpenStockApi.Core.Models.Searches;
using OpenStockApp.Core.Maui.Resources.Strings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserNotifications;

namespace OpenStockApp.Services.Notifications
{
    public class NotificationBuilder
    {
        private UNMutableNotificationContent notificationContent = new UNMutableNotificationContent();
        public NotificationBuilder WithResult(Result result)
        {
            notificationContent.Title = "In Stock";
            notificationContent.Body = string.Format(NotificationResources.ToastText, result.Sku?.Model?.Name ?? result.StockMessage);
            notificationContent.CategoryIdentifier = "url-category";
            notificationContent.UserInfo = new NSDictionary<NSString, NSString>(new NSString("url"), new NSString("url"));
            notificationContent.Sound = UNNotificationSound.Default;
            return this;
        }

        public UNMutableNotificationContent Build()
        {
            return notificationContent;
        }
    }
}