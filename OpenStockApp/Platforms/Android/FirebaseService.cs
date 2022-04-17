using Android.App;
using Android.Content;
#if ANDROID
using Android.Gms.Common;
using Android.OS;
using Android.Runtime;
using Firebase.Messaging;
using OpenStockApp;
using OpenStockApp.Platforms;
using OpenStockApp.Platforms.Android;
using System.Text.Json;

#endif
using Result = OpenStockApi.Core.Models.Searches.Result;

namespace OpenStockApp.Platforms.Android
{


#if ANDROID
    [Service(Name = "OpenStockApp.Platforms.Android.FirebaseService", DirectBootAware = true, Exported = false, Enabled = true)]
    [IntentFilter(new[] { "com.google.firebase.MESSAGING_EVENT" })]
    [IntentFilter(new[] { "com.google.firebase.INSTANCE_ID_EVENT" })]
    public class FirebaseService : FirebaseMessagingService
    {
        public FirebaseService(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
        {
            Console.WriteLine("Created");
        }
        
        public FirebaseService() : base()
        {
            Console.WriteLine("Created");
        }

        public override void OnNewToken(string p0)
        {
            base.OnNewToken(p0);
        }
        public override void OnMessageReceived(RemoteMessage remoteMessage)
        {
            base.OnMessageReceived(remoteMessage);

            if (remoteMessage.Data.Any())
            {
                if (remoteMessage.Data.TryGetValue("json", out string? rawJson) && remoteMessage.Data.TryGetValue("type", out string? type))
                {
                    switch (type)
                    {
                        case nameof(Result):
                            var result = JsonSerializer.Deserialize<Result>(rawJson);
                            if (result != null)
                                MessagingCenter.Send<FirebaseService, Result>(this, "NotificationReceived", result);
                            break;
                        default:
                            break;

                    }
                }
            }

            
        }

    }
#endif
}
