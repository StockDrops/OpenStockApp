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
#endif

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
        public override void OnMessageReceived(RemoteMessage p0)
        {
            base.OnMessageReceived(p0);
        }

    }
#endif
}
