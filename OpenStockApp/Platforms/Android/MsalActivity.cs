using Android.App;
using Android.Content;
using Android.OS;
using Microsoft.Identity.Client;
using OpenStockApp.Models;

namespace OpenStockApp.Platforms.Android
{
    [Activity(Exported = true)]
    [IntentFilter(new[] { Intent.ActionView },
        Categories = new[] { Intent.CategoryBrowsable, Intent.CategoryDefault },
        DataHost = "auth",
        DataScheme = $"msal{Secrets.ClientId}")] //TODO:Make these be the mobile app ids.
    public class MsalActivity : BrowserTabActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }
    }
}
