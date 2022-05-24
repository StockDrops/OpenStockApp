using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Gms.Common;
using Android.Gms.Extensions;
using Android.Gms.Tasks;
using Android.OS;
using Firebase.Installations;
using Firebase.Messaging;
using Microsoft.Identity.Client;
using OpenStockApi.Core.Models.Firebase;
//using Plugin.FirebasePushNotification;

namespace OpenStockApp;

[Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize)]
public class MainActivity : MauiAppCompatActivity, IOnCompleteListener
{
	protected override void OnCreate(Bundle savedInstanceState)
	{
		base.OnCreate(savedInstanceState);
		Platform.Init(this, savedInstanceState);


        if (IsPlayServicesAvailable(this))
        {
            CreateNotificationChannel(this);
        }
        //FirebaseMessaging.Instance.SubscribeToTopic(Topics.StockAlerts)
        //                          .AddOnCompleteListener(this);
        //var token = FirebaseInstallations.Instance.GetToken(forceRefresh: false).AsAsync<InstallationTokenResult>().Result;
        //Console.WriteLine("got it");

        //Set the default notification channel for your app when running Android Oreo
        //		if (Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.O)
        //		{
        //			//Change for your default notification channel id here
        //			FirebasePushNotificationManager.DefaultNotificationChannelId = "FirebasePushNotificationChannel";

        //			//Change for your default notification channel name here
        //			FirebasePushNotificationManager.DefaultNotificationChannelName = "General";
        //		}


        //		//If debug you should reset the token each time.
        //#if DEBUG
        //		FirebasePushNotificationManager.Initialize(this, true);
        //#else
        //        FirebasePushNotificationManager.Initialize(this,false);
        //#endif
        //		CrossFirebasePushNotification.Current.OnNotificationReceived += (s, p) =>
        //		{
        //			Console.WriteLine("Received");
        //		};

    }
	protected override void OnActivityResult(int requestCode, Result resultCode, Intent? data)
    {
        base.OnActivityResult(requestCode, resultCode, data);
		AuthenticationContinuationHelper.SetAuthenticationContinuationEventArgs(requestCode, resultCode, data);
	}
    public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Permission[] grantResults)
	{
		Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

		base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
	}

    internal static readonly string CHANNEL_ID = "my_notification_channel";
    public static bool IsPlayServicesAvailable(Context context)
    {
        return GoogleApiAvailability.Instance.IsGooglePlayServicesAvailable(context) == ConnectionResult.Success;
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

        var notificationManager = (NotificationManager)context.GetSystemService(NotificationService);

        // Don't re-create the notification channel if we already created it
        if (notificationManager is not null && notificationManager.GetNotificationChannel(CHANNEL_ID) == null)
        {
            var channel = new NotificationChannel(CHANNEL_ID,
                "FCM Notifications",
                NotificationImportance.Max);

            notificationManager.CreateNotificationChannel(channel);
        }
       
#pragma warning restore CA1416 // Validate platform compatibility
#else
            return;
#endif
    }

    public void OnComplete(Android.Gms.Tasks.Task task)
    {
        if (task.IsSuccessful)
        {

        }
    }
}
