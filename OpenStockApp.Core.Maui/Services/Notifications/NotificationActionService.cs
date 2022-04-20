using OpenStockApi.Core.Models.Searches;
using OpenStockApi.Core.Models.Users;
using OpenStockApp.Core.Contracts.Services;
using OpenStockApp.Core.Contracts.Services.Settings;

namespace OpenStockApp.Core.Maui.Services.Notifications
{
    public class NotificationActionService : INotificationService
    {
        private readonly IUserOptionsService userOptionsService;
        public NotificationActionService(IUserOptionsService userOptionsService)
        {
            this.userOptionsService = userOptionsService;
        }
        public async Task SendNotificationAsync(Result result)
        {
            try
            {
                if (result.Sku != null)
                {
                    var options = userOptionsService.GetModelOptionsFromCache(result.Sku.ModelId);
                    if (options is not null)
                    {
                        await Act(result, options.NotificationAction);
                    }
                }
            }
            catch(Exception ex)
            {

            }
            
        }
        private async Task Act(Result result, NotificationAction notificationAction)
        {
            switch (notificationAction)
            {
                case NotificationAction.OpenProductUrl:
                    await OpenUrl(result.ProductUrl);
                    break;
                case NotificationAction.OpenAddToCartUrl:
                    await OpenUrl(result.AtcUrl, result.ProductUrl);
                    break;
                case NotificationAction.DoNothing:
                    return;
            }
        }
        private async Task OpenUrl(string? url, string? backup = null)
        {
            if(url is not null)
            {
                await Browser.OpenAsync(url);
            }
            else if(backup is not null)
            {
                await Browser.OpenAsync(backup);
            }
        }

        public async Task SendNotificationAsync(IEnumerable<Result> results)
        {
            foreach(var result in results)
            {
                await SendNotificationAsync(result);
            }
        }

        public Task SendToastNotificationAsync(string text)
        {
            throw new NotImplementedException();
        }
    }
}
