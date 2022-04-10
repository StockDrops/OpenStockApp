using OpenStockApp.ViewModels.AlertSettings;

namespace OpenStockApp.Pages.Alerts;

public partial class ActiveNotificationsPage : ContentPage
{
	public ActiveNotificationsPage(ActiveNotificationsViewModel activeNotificationsViewModel)
	{
		BindingContext = activeNotificationsViewModel;
		InitializeComponent();
		
	}

}