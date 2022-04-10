using Microsoft.Maui;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenStockApp.Services
{
    public static class ServiceLocator
    {
        public static TService? GetService<TService>() => Current.GetService<TService>();
        public static TService GetRequiredService<TService>() where TService : notnull
        {
            return Current.GetRequiredService<TService>();
        }

        public static IServiceProvider Current =>
#if WINDOWS
            MauiWinUIApplication.Current.Services;
#elif MACCATALYST
            MauiUIApplicationDelegate.Current.Services;
#elif ANDROID
            MauiApplication.Current.Services;
        //null;
#else
        null;
#endif
    }
}
