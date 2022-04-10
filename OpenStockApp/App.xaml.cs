using OpenStockApp.Core.Maui.Services;
using OpenStockApp.Models;
using OpenStockApp.Pages;
using OpenStockApp.Services;

namespace OpenStockApp;

public partial class App : Application
{
	public App()
	{
        ThemeSelectorService.InitializeTheme();
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
