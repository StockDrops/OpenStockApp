using Foundation;
using Microsoft.Identity.Client;
using OpenStockApp.Models;
using UIKit;
using UserNotifications;

namespace OpenStockApp;

[Register("AppDelegate")]
public class AppDelegate : MauiUIApplicationDelegate, IUNUserNotificationCenterDelegate
{
	protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();
    public override bool OpenUrl(UIApplication application, NSUrl url, NSDictionary options)
    {
        AuthenticationContinuationHelper.SetAuthenticationContinuationEventArgs(url);
        return base.OpenUrl(application, url, options);
    }

    public override bool FinishedLaunching(UIApplication application, NSDictionary launchOptions)
    {
        UNUserNotificationCenter center = UNUserNotificationCenter.Current;
        var options = UNAuthorizationOptions.Alert | UNAuthorizationOptions.Sound;
        center.RequestAuthorization(options, (bool success, NSError error) =>
        {
            if (success)
            {
                center.Delegate = this;

                var openAddToCartUrl = UNNotificationAction.FromIdentifier(NotificationCategories.AddToCart, "Open Add to Cart Url", UNNotificationActionOptions.None);
                var openProductUrl = UNNotificationAction.FromIdentifier(NotificationCategories.ProductUrl, "Open Product Url", UNNotificationActionOptions.None);

                var openAllUrlCategory = UNNotificationCategory.FromIdentifier(
                    NotificationCategories.All,
                    new UNNotificationAction[] { openAddToCartUrl, openProductUrl },
                    new string[] { },
                    UNNotificationCategoryOptions.CustomDismissAction
                );

                var openAddToCartUrlCategory = UNNotificationCategory.FromIdentifier(
                    NotificationCategories.AddToCart,
                    new UNNotificationAction[] { openAddToCartUrl },
                    new string[] { },
                    UNNotificationCategoryOptions.CustomDismissAction
                    );

                var openProductUrlCategory = UNNotificationCategory.FromIdentifier(
                    NotificationCategories.ProductUrl,
                    new UNNotificationAction[] { openProductUrl },
                    new string[] { },
                    UNNotificationCategoryOptions.CustomDismissAction
                    );

                var set = new NSSet<UNNotificationCategory>(openAllUrlCategory, openAddToCartUrlCategory, openProductUrlCategory);
                center.SetNotificationCategories(set);
            }
        });
        center.RemoveAllPendingNotificationRequests();
        return base.FinishedLaunching(application, launchOptions);
    }

    [Export("userNotificationCenter:didReceiveNotificationResponse:withCompletionHandler:")]
    public async void DidReceiveNotificationResponse(UNUserNotificationCenter center, UNNotificationResponse response, System.Action completionHandler)
    {
        if (response.IsDefaultAction)
        {
            Console.WriteLine("ACTION: Default");
        }
        else if (response.IsCustomAction)
        {
            NSObject? url = null;
            switch (response.ActionIdentifier.ToString())
            {
                case (NotificationCategories.ProductUrl):
                    response.Notification.Request.Content.UserInfo.TryGetValue(new NSString(NotificationCategories.AddToCart), out url);
                    break;
                case (NotificationCategories.AddToCart):
                    response.Notification.Request.Content.UserInfo.TryGetValue(new NSString(NotificationCategories.ProductUrl), out url);
                    break;
            }
            if(url != null)
                await Browser.OpenAsync(url.ToString());
        }
        if (response.IsDismissAction)
        {
            Console.WriteLine("ACTION: Dismiss");
        }
        else
        {
            Console.WriteLine($"ACTION: {response.ActionIdentifier}");
        }

        completionHandler();
    }

    [Export("userNotificationCenter:willPresentNotification:withCompletionHandler:")]
    public void WillPresentNotification(UNUserNotificationCenter center, UNNotification notification, Action<UNNotificationPresentationOptions> completionHandler)
    {
        // Do something with the notification
        Console.WriteLine("Active Notification: {0}", notification);

        // Tell system to display the notification anyway or use
        // `None` to say we have handled the display locally.
        completionHandler(UNNotificationPresentationOptions.Alert | UNNotificationPresentationOptions.Sound);
    }
}
