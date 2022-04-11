using Foundation;
using OpenStockApi.Core.Models.Searches;
using OpenStockApp.Core.Maui.Resources.Strings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserNotifications;

namespace OpenStockApp.Core.Maui.Platforms.MacCatalyst.Notifications
{
    public class MacOsNotificaitonBuilder
    {
        private UNMutableNotificationContent notificationContent = new UNMutableNotificationContent();
        public MacOsNotificaitonBuilder WithResult(Result result)
        {
            notificationContent.Title = "In Stock";
            notificationContent.Body = string.Format(NotificationResources.ToastText, result.Sku.Model.Name);
            notificationContent.CategoryIdentifier = "openurl";
            notificationContent.UserInfo = new NSDictionary<NSString, NSString>(new NSString("url"), new NSString("url"));
            return this;
        }
        
        public UNMutableNotificationContent Build()
        {
            return notificationContent;
        }
    }
}
