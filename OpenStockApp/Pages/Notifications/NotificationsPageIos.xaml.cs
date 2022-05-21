using OpenStockApp.ViewModels;
using CommunityToolkit;
using CommunityToolkit.Maui.Behaviors;
using OpenStockApi.Core.Models.Searches;

namespace OpenStockApp.Pages.Alerts
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class NotificationsPageIos : ContentPage
    {
        
        public NotificationsPageIos(INotificationsPageViewModel notificationsPageViewModel)
        {
            BindingContext = notificationsPageViewModel;
            Behaviors.Add(new EventToCommandBehavior { EventName = nameof(this.NavigatedTo), Command = notificationsPageViewModel.NavigateToPage });
            InitializeComponent();
            //var converter = new CommunityToolkit.Maui.Converters.IsStringNotNullOrEmptyConverter();
             //TODO: One day I'd like to know why this is needed, and why the XAML binding is not working.
            //Behaviors.Add(new EventToCommandBehavior { EventName = nameof(this.Appearing), Command = notificationsPageViewModel.NavigateToPage });
            notificationsPageViewModel.ScrollTo = OnScrollTo;
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
