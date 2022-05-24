using Foundation;
using OpenStockApi.Core.Models.Searches;
using OpenStockApp.Core.Maui.Resources.Strings;
using OpenStockApp.Models;
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

            if (!string.IsNullOrEmpty(result.ProductUrl) && !string.IsNullOrEmpty(result.AtcUrl))
            {
                notificationContent.CategoryIdentifier = NotificationCategories.All;

                notificationContent.UserInfo = new NSDictionary<NSString, NSString>(
                    new NSString[]{
                        new NSString(NotificationCategories.ProductUrl),
                        new NSString(NotificationCategories.AddToCart)
                    },
                    new NSString[]
                    {
                        new NSString(result.ProductUrl),
                        new NSString(result.AtcUrl)
                    });
            }
            else if (!string.IsNullOrEmpty(result.ProductUrl))
            {
                notificationContent.CategoryIdentifier = NotificationCategories.ProductUrl;
                notificationContent.UserInfo = new NSDictionary<NSString, NSString>(
                        new NSString(NotificationCategories.ProductUrl),
                        new NSString(result.ProductUrl)
                    );
            }
            else if (!string.IsNullOrEmpty(result.AtcUrl))
            {
                notificationContent.CategoryIdentifier = NotificationCategories.AddToCart;
                notificationContent.UserInfo = new NSDictionary<NSString, NSString>(
                        new NSString(NotificationCategories.AddToCart),
                        new NSString(result.AtcUrl));
            }
            notificationContent.Sound = UNNotificationSound.Default;
            return this;
        }

        public UNMutableNotificationContent Build()
        {
            return notificationContent;
        }
    }
}