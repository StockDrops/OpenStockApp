using OpenStockApp.Core.Maui.Services;
using OpenStockApp.Models;
using OpenStockApp.Pages;
using OpenStockApp.Services;
using Microsoft.Maui.Platform;

#if ANDROID
using Android.Widget;
using Microsoft.Maui.Controls.Compatibility.Platform.Android;
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
            h.PlatformView.BackgroundTintList = ColorStateList.ValueOf(Colors.DarkGray.ToPlatform());

        });

        Microsoft.Maui.Handlers.PickerHandler.Mapper.AppendToMapping("UnderLinePicker", (h, v) =>
        {
            h.PlatformView.BackgroundTintList = ColorStateList.ValueOf(Colors.DarkGray.ToPlatform());
        });

        Microsoft.Maui.Handlers.SearchBarHandler.Mapper.AppendToMapping("LightSearch", (h, v) =>
        {

            var children = h.PlatformView.GetChildrenOfType<ImageView>();
            foreach(var child in children)
            {
                child.SetColorFilter(Colors.DarkGray.ToPlatform());
            }
            
            
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

    //void UpdateClearButtonColor(Microsoft.Maui.Handlers.ISearchBarHandler handler, ISearchBar searchBar)
    //{
    //    var id = handler.MauiContext?.Context?.Resources?.GetIdentifier("android:id/search_close_btn", null, null);
    //    if(id != null)
    //    {
    //        var resource = handler.PlatformView?.FindViewById(id.Value);

    //        var icon = resource as ImageView;
    //        if(icon != null)
    //        {
    //            if (!searchBar.PlaceholderColor.IsDefault())
    //                icon?.SetColorFilter(searchBar.PlaceholderColor.ToAndroid());
    //            else
    //                icon?.ClearColorFilter();
    //        }
    //    }
    //}

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
