using Microsoft.Maui.Dispatching;
using OpenStockApp.Core.Maui.Models.Users;
using OpenStockApp.ViewModels;
using OpenStockApp.ViewModels.Users;

namespace OpenStockApp.Pages;

public partial class LoginPage : ContentPage
{

	public LoginPage()
	{
		BindingContext = Services.ServiceLocator.GetRequiredService<LoginViewModel>();

        InitializeComponent();

        MessagingCenter.Subscribe<IIdentityViewModel, LoginResultType>(this, "LoggedIn", async (sender, result) =>
        {
            await Process(result).ConfigureAwait(false);
        });
    }

    private SemaphoreSlim semaphoreSlim = new SemaphoreSlim(1);
    public async Task Process(LoginResultType loginResultType)
    {
        await semaphoreSlim.WaitAsync();
        await Dispatcher.DispatchAsync(async () => await ProcessLoginResult(loginResultType));
        semaphoreSlim.Release();
    }

    public async Task ProcessLoginResult(LoginResultType loginResultType)
    {
        try
        {
            CommunityToolkit.Maui.Alerts.Toast? toast = null;
            switch (loginResultType)
            {
                case LoginResultType.Success:
                    
                    //await (Shell.Current?.Navigation?.PopModalAsync(true) ?? Task.CompletedTask).ConfigureAwait(false);
                    break;
                case LoginResultType.NoNetworkAvailable:
                    toast = new CommunityToolkit.Maui.Alerts.Toast { Text = OpenStockApp.Resources.Strings.Resources.DialogNoNetworkAvailableContent, Duration = CommunityToolkit.Maui.Core.ToastDuration.Long };
                    //await toast.Show();
                    break;
                case LoginResultType.CancelledByUser:
                    //toast = new CommunityToolkit.Maui.Alerts.Toast { Text = "Cancelled", Duration = CommunityToolkit.Maui.Core.ToastDuration.Long };
                    //await DisplayAlert("Cancelled", "Cancelled", "Ok");
                    break;
                case LoginResultType.Unauthorized:
                case LoginResultType.UnknownError:
                default:
                    toast = new CommunityToolkit.Maui.Alerts.Toast { Text = OpenStockApp.Resources.Strings.Resources.DialogStatusUnknownErrorContent, Duration = CommunityToolkit.Maui.Core.ToastDuration.Long };
                    //await toast.Show();
                    break;
            }
            await (toast?.Show() ?? Task.CompletedTask);
        }
        catch(Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"{ex}");
        }
        
    }
}