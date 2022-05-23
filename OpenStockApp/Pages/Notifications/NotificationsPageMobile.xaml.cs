using CommunityToolkit;
using CommunityToolkit.Maui.Behaviors;
using OpenStockApi.Core.Models.Searches;
using OpenStockApp.ViewModels.Notifications;

namespace OpenStockApp.Pages.Alerts
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class NotificationPageMobile : ContentPage
    {
        
        public NotificationPageMobile(NotificationsPageViewModel notificationsPageViewModel)
        {
            BindingContext = notificationsPageViewModel;
            InitializeComponent();
            
            //var converter = new CommunityToolkit.Maui.Converters.IsStringNotNullOrEmptyConverter();
            Behaviors.Add(new EventToCommandBehavior { EventName = nameof(this.Appearing), Command = notificationsPageViewModel.NavigateToPage }); //TODO: One day I'd like to know why this is needed, and why the XAML binding is not working.
            //Behaviors.Add(new EventToCommandBehavior { EventName = nameof(this.Appearing), Command = notificationsPageViewModel.NavigateToPage });
            notificationsPageViewModel.ScrollTo = OnScrollTo;
            //notificationsPageViewModel.SetBinding(NotificationsPageViewModel.ListHeightProperty, new Binding(nameof(Height), source: collectionView));
        }

        
        public void OnScrollTo(Result result)
        {
#if ANDROID || IOS
            try
            {
                if (result != null)
                    collectionView?.ScrollTo(0, position: ScrollToPosition.Start);
            }
            catch { }
#endif  
        }
    }
}
