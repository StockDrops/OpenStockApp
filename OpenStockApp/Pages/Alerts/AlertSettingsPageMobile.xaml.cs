using OpenStockApp.ViewModels.AlertSettings;
using CommunityToolkit.Maui.Behaviors;
using OpenStockApp.Services;
using StringResources = OpenStockApp.Resources.Strings.Resources;
using System.Windows.Input;

namespace OpenStockApp.Pages.Alerts;

public partial class AlertSettingsPageMobile : ContentPage 
{    
    public AlertSettingsPageMobile(AlertSettingsViewModel alertSettingsViewModel)
    {        
        BindingContext = alertSettingsViewModel;
#if ANDROID || IOS || WINDOWS
        DisplayHelp = new Command(() => OnDisplayHelp());
#endif
        InitializeComponent(); 

        Behaviors.Add(new EventToCommandBehavior { EventName = nameof(this.Appearing), Command = alertSettingsViewModel.LoadProducts });
        //Behaviors.Add(new EventToCommandBehavior { EventName = nameof(this.NavigatedFrom), Command = alertSettingsViewModel.NavigatedAwayCommand });
        MessagingCenter.Subscribe<AlertSettingsViewModel, Exception?>(this, "saved", async (sender, args) =>
        {
            await OnSaved(sender, args);
        });
    }
    public async Task OnSaved(object sender, object? exception)
    {
        if (exception is null)
            await DisplayAlert("Saved", "Alert settings saved", "Ok");
        else if (exception is HubNotConnected)
            await DisplayAlert("Failed to Save", "Alerts settings were not saved since you are not connected to our server.", "Ok");
    }
    public ICommand DisplayHelp { get; set; }
    public void OnDisplayHelp()
    {
        DisplayAlert(StringResources.AddNotificationsExplanationTitle, StringResources.AddNotificationsExplanation, "Ok");
    }
}
