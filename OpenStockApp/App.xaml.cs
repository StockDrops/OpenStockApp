using OpenStockApp.Core.Maui.Services;
using OpenStockApp.Models;
using OpenStockApp.Pages;
using OpenStockApp.Services;
using Microsoft.Maui.Platform;
#if ANDROID
using Android.Content.Res;
#endif
namespace OpenStockApp;

public partial class App : Application
{
	public App()
	{
        ThemeSelectorService.InitializeTheme();

#if ANDROID
        Microsoft.Maui.Handlers.EntryHandler.Mapper.AppendToMapping("LightUnderline", (h, v) =>
        {
            h.PlatformView.BackgroundTintList = ColorStateList.ValueOf(Colors.LightGray.ToPlatform());

        });
        
        Microsoft.Maui.Handlers.SearchBarHandler.Mapper.AppendToMapping("LightSearch", (h, v) =>
        {
            //h.PlatformView.SetBackgroundColor(Colors.LightGray.ToPlatform());
            //h.PlatformView.BackgroundTintList = ColorStateList.ValueOf(Colors.Blue.ToPlatform());
            //h.PlatformView.ForegroundTintList = ColorStateList.ValueOf(Colors.Blue.ToPlatform());

            
            h.PlatformView.SetOutlineAmbientShadowColor(Colors.Violet.ToPlatform());
            h.PlatformView.SetOutlineSpotShadowColor(Colors.DarkGoldenrod.ToPlatform());
            //h.PlatformView.BackgroundTintList = ColorStateList.ValueOf(Colors.LightGray.ToPlatform());
        });
#endif


        InitializeComponent();
        
        if (AppConfig.Desktop)
            MainPage = new DesktopShell();
        else
            MainPage = new MobileShell();
        //MainPage = new MainPage();
        //RegisterLifecycleEvents();
    }
    protected override Window CreateWindow(IActivationState? activationState)
    {
        var window = base.CreateWindow(activationState);

        //window.Created += (s, e) => BackgroundServicesContainer.StartApp();

        return window;
    }
}
