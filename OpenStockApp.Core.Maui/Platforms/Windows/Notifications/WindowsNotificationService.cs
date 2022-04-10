using Microsoft.Extensions.Logging;
using OpenStockApi.Core.Models.Searches;
using OpenStockApp.Core.Contracts.Services;
using OpenStockApp.Core.Maui.Platforms.Windows.Notifications;
using Microsoft.Toolkit.Uwp.Notifications;
using System.Web;

namespace OpenStockApp.Core.Maui.Platforms.Windows
{
    public class WindowsNotificationService : INotificationService
    {
        private readonly ILogger<WindowsNotificationService> logger;
        public WindowsNotificationService(ILogger<WindowsNotificationService> logger)
        {
            this.logger = logger;
        }

        public event EventHandler<Result>? OnResultReceived;

        public async static void OnActivated(ToastNotificationActivatedEventArgsCompat toastArgs)
        {
            try
            {
                ToastArguments args = ToastArguments.Parse(toastArgs.Argument);
                if (args.TryGetValue("url", out var url))
                {
                    await Browser.OpenAsync(HttpUtility.UrlDecode(url));
                }
            }
            catch { }
        }
       
        public Task SendNotificationAsync(Result result)
        {
            if(result.Sku != null)
            {

                Application.Current?.Dispatcher.Dispatch(() =>
                {
                    ShowPlatform(result);
                });
                
            }
            OnResultReceived?.Invoke(this, result);
            return Task.CompletedTask;
        }

        protected void ShowPlatform(Result result)
        {
            try
            {
                var builder = new NotificationBuilder();
                builder.WithResult(result);
                builder.Show();
            }
            catch(Exception ex)
            {
                logger.LogError(ex, "");
            }
            
        }

        public Task SendNotificationAsync(IEnumerable<Result> results)
        {
            foreach(var result in results)
            {
                SendNotificationAsync(result);
            }
            return Task.CompletedTask;
        }

        public Task SendToastNotificationAsync(string text)
        {
            var builder = new NotificationBuilder();
            builder.WithText(text);
            builder.Show();
            return Task.CompletedTask;
        }
    }
}
