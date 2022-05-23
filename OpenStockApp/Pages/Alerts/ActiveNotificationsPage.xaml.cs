using CommunityToolkit.Maui.Behaviors;
using OpenStockApp.ViewModels.AlertSettings;
using System.Windows.Input;
using StringResources = OpenStockApp.Resources.Strings.Resources;

namespace OpenStockApp.Pages.Alerts;

public partial class ActiveNotificationsPage : ContentPage
{
	public ActiveNotificationsPage(ActiveNotificationsViewModel activeNotificationsViewModel)
	{

		//Behaviors.Add(new EventToCommandBehavior { EventName = nameof(this.Focused), Command = activeNotificationsViewModel.NavigatedToCommand });
		BindingContext = activeNotificationsViewModel;
#if ANDROID || IOS || WINDOWS
		DisplayHelp = new Command(() => OnDisplayHelp());
#endif
		InitializeComponent();
#if ANDROID //this is trying to workaround the events not working as expected.
		MessagingCenter.Send(this, "NavigatedTo");
#endif
		//NavigatedTo += OnNavigatedTo; //These are not getting triggered. We need to see if it's still the case in RC1 and open a bug report.
		//Appearing += OnNavigatedTo;

	}
	public ICommand DisplayHelp { get; set; }
	public void OnDisplayHelp()
    {
		DisplayAlert(StringResources.ActiveNotificationsExplanationTitle, StringResources.ActiveNotificationsExplanation, "Ok");
    }
}