using CoreFoundation;
using Foundation;
using Microsoft.Identity.Client;
using Microsoft.Maui.Dispatching;
using OpenStockApp.Core.Contracts.Services.Settings;
using OpenStockApp.Core.Contracts.Services.Users;
using OpenStockApp.Core.Models.Events;
using OpenStockApp.Discord.Contracts.Services;
using OpenStockApp.Models;
using OpenStockApp.Platforms.iOS.Helpers;
using OpenStockApp.Services;
using OpenStockApp.Services.Notifications;
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

                var openAddToCartUrl = UNNotificationAction.FromIdentifier(NotificationCategories.AddToCart, "Open Add to Cart Url", UNNotificationActionOptions.Foreground);
                var openProductUrl = UNNotificationAction.FromIdentifier(NotificationCategories.ProductUrl, "Open Product Url", UNNotificationActionOptions.Foreground);

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

        UIApplication.SharedApplication.RegisterForRemoteNotifications();


        return base.FinishedLaunching(application, launchOptions);
    }
    /// <summary>
    /// This gets called when the app succesfully registers with APNS. This will send the token to AWS.
    /// </summary>
    /// <param name="application"></param>
    /// <param name="deviceToken"></param>
    [Export("application:didRegisterForRemoteNotificationsWithDeviceToken:")]
    public virtual void RegisteredForRemoteNotifications(UIKit.UIApplication application, Foundation.NSData deviceToken)
    {
        System.Diagnostics.Debug.WriteLine($"Device Name: {DeviceInfo.Name}");
        var hexString = Convert.ToHexString(Convert.FromBase64String(deviceToken.GetBase64EncodedString(NSDataBase64EncodingOptions.None))).ToLower();
        System.Diagnostics.Debug.WriteLine($"Token: {hexString}");
        MessagingCenter.Send(this, Events.RegisterToken, hexString);
    }
    /// <summary>
    /// This will receive the notification and send it.
    /// </summary>
    /// <param name="application"></param>
    /// <param name="userInfo"></param>
    /// <param name="completionHandler"></param>
    [Export("application:didReceiveRemoteNotification:fetchCompletionHandler:")]
    public async void DidReceiveRemoteNotification(UIApplication application, NSDictionary userInfo, Action<UIKit.UIBackgroundFetchResult> completionHandler)
    {
        try
        {
            var dict = userInfo.ToDictionary();
            var decoded = NotificationDecoderService.DecodeNotification(dict);
            var discordService = ServiceLocator.GetService<IDiscordWebhookService>();
            if (discordService is not null && decoded is not null)
            {
                await discordService.ExecuteDiscordWebhook(decoded).ConfigureAwait(false);
                completionHandler(UIBackgroundFetchResult.NewData);
                return;
            }
            completionHandler(UIBackgroundFetchResult.NoData);
            return;
        }
        finally
        {
            completionHandler(UIBackgroundFetchResult.Failed);
        }
        
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
                    response.Notification.Request.Content.UserInfo.TryGetValue(new NSString(NotificationCategories.ProductUrl), out url);
                    break;
                case (NotificationCategories.AddToCart):
                    response.Notification.Request.Content.UserInfo.TryGetValue(new NSString(NotificationCategories.AddToCart), out url);
                    break;
            }
            switch (UIApplication.SharedApplication.ApplicationState)
            {
                case UIApplicationState.Background:
                    break;
                default:
                    if(url is not null)
                        Open(url.ToString());
                    break;
            }

            //if(url != null)
            //{
            //    DispatchQueue.MainQueue.DispatchAsync(async () =>
            //    {
            //        var appleUrl = new NSUrl(url.ToString());
            //        //await Launcher.OpenAsync(new Uri(url.ToString()));
            //        //var ok = UIApplication.SharedApplication.OpenUrl(appleUrl);
            //        //System.Diagnostics.Debug.WriteLine($"Opened: {ok}");
            //        if(UIApplication.SharedApplication.CanOpenUrl(new NSUrl("googlechrome://")))
            //        {
            //            UIApplication.SharedApplication.OpenUrl(new NSUrl("googlechrome://google.com"), new UIApplicationOpenUrlOptions(), (ok) =>
            //            {
            //                System.Diagnostics.Debug.WriteLine($"Opened: {ok}");
            //            });
            //        }
                    
            //    });
            //    if (UIApplication.SharedApplication.CanOpenUrl(new NSUrl("googlechrome://")))
            //    {
            //        UIApplication.SharedApplication.OpenUrl(new NSUrl("googlechrome://google.com"), new UIApplicationOpenUrlOptions(), (ok) =>
            //        {
            //            System.Diagnostics.Debug.WriteLine($"Opened: {ok}");
            //        });
            //    }

            //}
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

    private void Open(string url)
    {
        
        
        if (UIApplication.SharedApplication.CanOpenUrl(new NSUrl("googlechrome://")))
        {
            var uri = new Uri(url);
            UIApplication.SharedApplication.OpenUrl(new NSUrl($"googlechrome://{uri.Host + uri.PathAndQuery + uri.Fragment}"), new UIApplicationOpenUrlOptions(), (ok) =>
            {
                System.Diagnostics.Debug.WriteLine($"Opened on chrome: {ok}");
            });
        }
        else
        {
            var appleUrl = new NSUrl(url);
            UIApplication.SharedApplication.OpenUrl(appleUrl, new UIApplicationOpenUrlOptions(), (ok) =>
            {
                System.Diagnostics.Debug.WriteLine($"Opened on default browser: {ok}");
            });
        }
    }

    [Export("userNotificationCenter:willPresentNotification:withCompletionHandler:")]
    public void WillPresentNotification(UNUserNotificationCenter center, UNNotification notification, Action<UNNotificationPresentationOptions> completionHandler)
    {
        // Do something with the notification
        Console.WriteLine("Active Notification: {0}", notification);

        // Tell system to display the notification anyway or use
        // `None` to say we have handled the display locally.
        completionHandler(UNNotificationPresentationOptions.Badge | UNNotificationPresentationOptions.Sound);
    }
}
