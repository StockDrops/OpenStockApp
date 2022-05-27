using OpenStockApp.ViewModels;
using CommunityToolkit;
using CommunityToolkit.Maui.Behaviors;
using OpenStockApi.Core.Models.Searches;
using OpenStockApp.Services;

namespace OpenStockApp.Pages.Alerts
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class NotificationsPageIos : ContentPage
    {
        public readonly INotificationsPageViewModel notificationsPageViewModel;
        public NotificationsPageIos(INotificationsPageViewModel notificationsPageViewModel)
        {
            BindingContext = notificationsPageViewModel;
            this.notificationsPageViewModel = notificationsPageViewModel;
            Behaviors.Add(new EventToCommandBehavior { EventName = nameof(this.NavigatedTo), Command = notificationsPageViewModel.NavigateToPage });
            InitializeComponent();
            //var converter = new CommunityToolkit.Maui.Converters.IsStringNotNullOrEmptyConverter();
             //TODO: One day I'd like to know why this is needed, and why the XAML binding is not working.
            //Behaviors.Add(new EventToCommandBehavior { EventName = nameof(this.Appearing), Command = notificationsPageViewModel.NavigateToPage });
            notificationsPageViewModel.ScrollTo = OnScrollTo;
            
        }
        
        protected async override void OnAppearing()
        {
            base.OnAppearing();

            if (!notificationsPageViewModel.IsLoggedIn)
            {
                if (Shell.Current?.Navigation?.ModalStack.Any() == false)
                {
#if WINDOWS
                    await Task.Delay(1000);
#endif
                    //there's no modal already in the modal stack.
                    await (Shell.Current?.Navigation?.PushModalAsync(new LoginPage(), false) ?? Task.CompletedTask).ConfigureAwait(false);
                } 
            }

        }

        public void OnScrollTo(Result result)
        {
#if ANDROID || IOS
            try
            {
                //if (result != null)
                //    collectionView?.ScrollTo(0, position: ScrollToPosition.Start);
            }
            catch { }
#endif
        }
    }
}
