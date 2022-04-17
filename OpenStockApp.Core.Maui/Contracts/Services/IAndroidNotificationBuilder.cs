#if ANDROID
using Android.App;

namespace OpenStockApp.Core.Maui.Contracts.Services
{
    public interface IAndroidNotificationBuilder
    {
        Notification CreateNotification(OpenStockApi.Core.Models.Searches.Result result);
    }
}
#endif