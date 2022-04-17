using Android.App;
using Android.Content;
using Android.OS;
using AndroidX.Core.App;
using Microsoft.Extensions.Logging;
using OpenStockApi.Core.Models.Searches;
using OpenStockApp.Core.Contracts.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Result = OpenStockApi.Core.Models.Searches.Result;
using Uri = Android.Net.Uri;

namespace OpenStockApp.Maui.Platforms.Android
{
    public class AndroidNotificationService : INotificationService
    {
        private readonly ILogger<AndroidNotificationService> logger;
        public AndroidNotificationService(ILogger<AndroidNotificationService> logger)
        {
            this.logger = logger;
        }
        private int notificationId = 0;
        public Task SendNotificationAsync(Result result)
        {
            var notification = CreateNotification(result);
            try
            {
                NotificationManagerCompat notificationManager = NotificationManagerCompat.From(Platform.AppContext);
                notificationManager.Notify(notificationId, notification);
                notificationId++;
            }
            catch(Exception ex)
            {
                logger.LogError(ex, "");
            }
            return Task.CompletedTask;
        }

        public async Task SendNotificationAsync(IEnumerable<Result> results)
        {
            foreach (var result in results)
            {
                await SendNotificationAsync(result);
            }
        }
        private const string CHANNEL_ID = "my_notification_channel";
        private Notification CreateNotification(Result result)
        {
            CreateNotificationChannel(Platform.AppContext);
            NotificationCompat.Builder notificationBuilder = new NotificationCompat.Builder(Platform.AppContext, CHANNEL_ID)
                .SetSmallIcon(Resource.Drawable.sd_icon)
                .SetColor(Colors.Black.ToInt());
            if (result.Sku?.Model?.Name != null)
            {
                notificationBuilder.SetContentTitle($"{result.Sku.Model.Name} - {result.StockMessage}");
            }
            var text = "";
            if (result.Sku?.Retailer is not null)
            {
                text += $"Available at {result.Sku.Retailer.Name}. ";
            }
            if (result.Price != null)
            {
                text += $"{result.Price}";
                if (result.PriceMetrics?.MinSeenPrice?.Price is not null && result.Price.Value <= result.PriceMetrics.MinSeenPrice.Price.Value)
                {
                    text += $"Best Price since {result.PriceMetrics.MinSeenPrice.DateTimeSeen:M}";
                }
            }
            notificationBuilder.SetContentText(text)
                .SetPriority(NotificationCompat.PriorityDefault);
            if (!string.IsNullOrEmpty(result.AtcUrl))
            {
                Intent addToCartIntent = new Intent(Intent.ActionView, Uri.Parse(result.AtcUrl));
                PendingIntent? pendingIntent = PendingIntent.GetActivity(Platform.AppContext, 0, addToCartIntent, PendingIntentFlags.OneShot);
                notificationBuilder.AddAction(0, Resources.Strings.Resources.AtcUrlButtonText, pendingIntent);
            }
            if (!string.IsNullOrEmpty(result.ProductUrl))
            {
                Intent productUrlIntent = new Intent(Intent.ActionView, Uri.Parse(result.AtcUrl));
                PendingIntent? pendingIntent = PendingIntent.GetActivity(Platform.AppContext, 0, productUrlIntent, PendingIntentFlags.OneShot);
                notificationBuilder.AddAction(0, Resources.Strings.Resources.ProductUrlButtonText, pendingIntent);
            }
            return notificationBuilder.Build();


        }

        public static void CreateNotificationChannel(Context context)
        {
#if ANDROID26_0_OR_GREATER
            if (Build.VERSION.SdkInt < BuildVersionCodes.O)
            {
                // Notification channels are new in API 26 (and not a part of the
                // support library). There is no need to create a notification
                // channel on older versions of Android.
                return;
            }

#pragma warning disable CA1416 // Validate platform compatibility, this is not reachable in API 26 <

            var notificationManager = (NotificationManager?)context.GetSystemService(Context.NotificationService);

            // Don't re-create the notification channel if we already created it
            if (notificationManager is not null && notificationManager.GetNotificationChannel(CHANNEL_ID) == null)
            {
                var channel = new NotificationChannel(CHANNEL_ID,
                    "FCM Notifications",
                    NotificationImportance.Max);

                notificationManager.CreateNotificationChannel(channel);
            }
#endif
        }
    }
}
