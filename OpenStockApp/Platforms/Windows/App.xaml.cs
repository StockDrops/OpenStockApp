using Microsoft.UI.Xaml;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace OpenStockApp.WinUI;

/// <summary>
/// Provides application-specific behavior to supplement the default Application class.
/// </summary>
public partial class App : MauiWinUIApplication
{
    /// <summary>
    /// Initializes the singleton application object.  This is the first line of authored code
    /// executed, and as such is the logical equivalent of main() or WinMain().
    /// </summary>
    public App()
    {
        //TODO: Remove this once they fixed the shell bug without a title bar.
//        Microsoft.Maui.Handlers.WindowHandler.WindowMapper.AppendToMapping(nameof(IWindow), (h, v) =>
//        {
//#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
//#pragma warning disable CS8602 // Dereference of a possibly null reference.
//            ((MauiWinUIWindow)v.Handler.NativeView)!.ExtendsContentIntoTitleBar = false;
//#pragma warning restore CS8602 // Dereference of a possibly null reference.
//#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.
//        });
        this.InitializeComponent();
    }

    protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();

    protected override void OnLaunched(LaunchActivatedEventArgs args)
    {
        base.OnLaunched(args);

        Platform.OnLaunched(args);
    }
}

