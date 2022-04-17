using Android.App;
using Android.Content;
using Android.OS;
using AndroidX.Core.App;
using Microsoft.Extensions.Logging;
using OpenStockApi.Core.Models.Searches;
using OpenStockApp.Core.Contracts.Services;
using OpenStockApp.Core.Maui.Contracts.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Result = OpenStockApi.Core.Models.Searches.Result;
using Uri = Android.Net.Uri;

namespace OpenStockApp.Platforms.Android
{
    public class AndroidNotificationService : INotificationService
    {
        private readonly ILogger<AndroidNotificationService> logger;
        private readonly IAndroidNotificationBuilder androidNotificationBuilder;
        public AndroidNotificationService(IAndroidNotificationBuilder androidNotificationBuilder, ILogger<AndroidNotificationService> logger)
        {
            this.androidNotificationBuilder = androidNotificationBuilder;
            this.logger = logger;
        }

        //private async Task OnNotificationReceived(FirebaseService sender, Result remoteMessage)
        //{
        //   await SendNotificationAsync(remoteMessage);
        //}
        public Task SendNotificationAsync(Result result)
        {
            var notification = androidNotificationBuilder.CreateNotification(result);
            try
            {
                if(notification != null)
                {
                    NotificationManagerCompat notificationManager = NotificationManagerCompat.From(Platform.AppContext);
                    notificationManager.Notify(unchecked((int)result.Id), notification);
                }
            }
            catch (Exception ex)
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
    }
}
