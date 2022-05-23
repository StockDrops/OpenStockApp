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

#if ANDROID28_0_OR_GREATER
            h.PlatformView.SetOutlineAmbientShadowColor(Colors.Violet.ToPlatform());
            h.PlatformView.SetOutlineSpotShadowColor(Colors.DarkGoldenrod.ToPlatform());
#endif
            //h.PlatformView.BackgroundTintList = ColorStateList.ValueOf(Colors.LightGray.ToPlatform());
        });
#elif IOS
        Microsoft.Maui.Handlers.SearchBarHandler.Mapper.AppendToMapping("NoBackground", (h, v) =>
        {
            h.PlatformView.SearchBarStyle = UIKit.UISearchBarStyle.Minimal;
            //h.PlatformView.SearchTextField.BackgroundColor = Colors.Red.ToPlatform();
            //h.PlatformView.BarTintColor = Colors.Blue.ToPlatform();
        });
        Microsoft.Maui.Handlers.PickerHandler.Mapper.AppendToMapping("NoBackground", (h, v) =>
        {
            //h.PlatformView.BorderStyle = UIKit.UITextBorderStyle.RoundedRect;
            h.PlatformView.BorderStyle = UIKit.UITextBorderStyle.None;
            
            //h.PlatformView.BackgroundColor = UIKit.UIColor.SystemGray5;
            //h.PlatformView.UIPickerView.BackgroundColor = UIKit.UIColor.SystemGray; //.BorderStyle = UIKit.UITextBorderStyle.RoundedRect;
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

        window.Resumed += (s, e) =>
        {
#if ANDROID || IOS
            BackgroundServicesContainer.StartApp();
#endif
        };
        window.Stopped += (s, e) =>
        {
#if ANDROID || IOS
            BackgroundServicesContainer.StopApp();
#endif
        };
        

        return window;
    }
}
