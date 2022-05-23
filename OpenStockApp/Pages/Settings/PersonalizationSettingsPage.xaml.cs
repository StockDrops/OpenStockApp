using OpenStockApp.ViewModels.Settings;

namespace OpenStockApp.Pages.Settings;

[XamlCompilation(XamlCompilationOptions.Compile)]
public partial class PersonalizationSettingsPage : ContentPage
{
    public PersonalizationSettingsPage(PersonalizationViewModel viewModel)
    {
        BindingContext = viewModel;
        InitializeComponent();
        
        //BindingContext = Maui.Services.Application.ServiceProvider.GetService<AboutSettingsViewModel>();
    }
}
