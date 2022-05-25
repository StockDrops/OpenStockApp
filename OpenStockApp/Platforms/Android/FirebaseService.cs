using Android.App;
using Android.Content;
#if ANDROID
using Android.Gms.Common;
using Android.OS;
using Android.Runtime;
using Firebase.Messaging;
using OpenStockApp;
using OpenStockApp.Core.Models.Events;
using OpenStockApp.Platforms;
using OpenStockApp.Platforms.Android;
using System.IO.Compression;
using System.Text;
using System.Text.Json;

#endif
using Result = OpenStockApi.Core.Models.Searches.Result;

namespace OpenStockApp.Platforms.Android
{


#if ANDROID
    [Service(Name = "OpenStockApp.Platforms.Android.FirebaseService", DirectBootAware = true, Exported = false, Enabled = true)]
    [IntentFilter(new[] { "com.google.firebase.MESSAGING_EVENT" })]
    //[IntentFilter(new[] { "com.google.firebase.INSTANCE_ID_EVENT" })]
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
            System.Diagnostics.Debug.WriteLine($"Device Name: {DeviceInfo.Name}");
            System.Diagnostics.Debug.WriteLine($"Token: {p0}");
            MessagingCenter.Send(this, Events.RegisterToken, p0);
            base.OnNewToken(p0);
        }
        public override void OnMessageReceived(RemoteMessage remoteMessage)
        {
            base.OnMessageReceived(remoteMessage);
            

            if (remoteMessage.Data.Any())
            {
                if (remoteMessage.Data.TryGetValue("json", out string? rawBytes) && remoteMessage.Data.TryGetValue("type", out string? type) && remoteMessage.Data.TryGetValue("encoding", out var encoding))
                {
                    var rawJson = UncompressMessage(rawBytes, encoding);

                    if (rawJson is null)
                        return;

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

        private string? UncompressMessage(string base64string, string encoding)
        {
            var bytes = Convert.FromBase64String(base64string);
            switch (encoding)
            {
                case "br":
                    return UncompressBrotli(bytes);
                default:
                    throw new ArgumentException("Unkwnown encoding");
            }
        }

        private string? UncompressBrotli(Span<byte> bytes)
        {
            var span = new Span<byte>(new byte[4 * bytes.Length]);
            if(BrotliDecoder.TryDecompress(bytes, span, out int bytesWritten))
            {
                return Encoding.UTF8.GetString(span.Slice(0, bytesWritten));
            }
            return null;
        }

    }
#endif
}
