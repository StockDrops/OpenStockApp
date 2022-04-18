using OpenStockApp.ViewModels.AlertSettings;

namespace OpenStockApp.Pages.Alerts;

public partial class RetailerOptionsPage : ContentPage
{
	public RetailerOptionsPage(RetailerOptionsViewModel retailerOptionsViewModel)
	{
		BindingContext = retailerOptionsViewModel;
		InitializeComponent();
		MessagingCenter.Send(this, "NavigatedTo");
	}
}