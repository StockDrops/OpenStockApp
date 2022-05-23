using Microsoft.Toolkit.Mvvm.Input;
using OpenStockApp.Services;
using OpenStockApp.ViewModels.AlertSettings;
using System.Windows.Input;
using StringResources = OpenStockApp.Resources.Strings.Resources;

namespace OpenStockApp.Pages.Alerts;

public partial class RetailerOptionsPage : ContentPage
{
	public RetailerOptionsPage(RetailerOptionsViewModel retailerOptionsViewModel)
	{
		BindingContext = retailerOptionsViewModel;
#if ANDROID || IOS || WINDOWS
        DisplayHelp = new AsyncRelayCommand(OnDisplayHelp);
#endif
        InitializeComponent();
        
#if ANDROID
        MessagingCenter.Send(this, "NavigatedTo");
#endif
        MessagingCenter.Subscribe<RetailerOptionsViewModel, Exception?>(this, "saved", async (sender, args) =>
        {
            await OnSaved(sender, args);
        });

    }
    public async Task OnSaved(object? sender, object? exception)
    {
        if (exception is null)
            await DisplayAlert("Saved", "Alert settings saved", "Ok");
        else if (exception is HubNotConnected)
            await DisplayAlert("Failed to Save", "Alerts settings were not saved since you are not connected to our server.", "Ok");
    }
    public ICommand DisplayHelp { get; set; }
    public async Task OnDisplayHelp(CancellationToken cancellationToken = default)
    {
        await DisplayAlert(StringResources.AddNotificationsExplanationTitle, StringResources.AddNotificationsExplanation, "Ok");
    }
}