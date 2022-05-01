using Foundation;
using Microsoft.Identity.Client;
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
            center.Delegate = this;

            var openAddToCartUrl = UNNotificationAction.FromIdentifier("atcurl", "Open Add to Cart Url", UNNotificationActionOptions.None);
            var openProductUrl = UNNotificationAction.FromIdentifier("producturl", "Open Product Url", UNNotificationActionOptions.None);

            var openUrlCategory = UNNotificationCategory.FromIdentifier(
                "url-category",
                new UNNotificationAction[] { openAddToCartUrl, openProductUrl },
                new string[] { },
                UNNotificationCategoryOptions.CustomDismissAction
            );

            var set = new NSSet<UNNotificationCategory>(openUrlCategory);
            center.SetNotificationCategories(set);
        });
        center.RemoveAllPendingNotificationRequests();
        return base.FinishedLaunching(application, launchOptions);
    }

    [Export("userNotificationCenter:didReceiveNotificationResponse:withCompletionHandler:")]
    public void DidReceiveNotificationResponse(UNUserNotificationCenter center, UNNotificationResponse response, System.Action completionHandler)
    {
        if (response.IsDefaultAction)
        {
            Console.WriteLine("ACTION: Default");
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
}
