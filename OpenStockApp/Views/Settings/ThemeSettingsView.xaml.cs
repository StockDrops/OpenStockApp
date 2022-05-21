using OpenStockApp.Services;
using OpenStockApp.ViewModels.Settings;
namespace OpenStockApp.Views.Settings;

//[XamlCompilation(XamlCompilationOptions.Compile)]
public partial class ThemeSettingsView : ContentView
{
    public ThemeSettingsView()
    {
        InitializeComponent();
        BindingContext = ServiceLocator.GetRequiredService<ThemeViewModel>();
    }
}
