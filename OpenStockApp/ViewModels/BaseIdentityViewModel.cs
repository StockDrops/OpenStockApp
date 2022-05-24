using Microsoft.Toolkit.Mvvm.Input;
using OpenStockApp.Core.Contracts.Services.Hubs;
using OpenStockApp.Core.Contracts.Services.Users;
using OpenStockApp.Core.Maui.Models.Users;
using OpenStockApp.Pages;
using OpenStockApp.Services;
using OpenStockApp.ViewModels.Notifications;
using System.Windows.Input;

namespace OpenStockApp.ViewModels
{
    public class BaseIdentityViewModel : BindableBaseViewModel, IDisposable, IIdentityViewModel
    {

        public BindableProperty IsLoggedInProperty = BindableProperty.Create(nameof(IsLoggedIn), typeof(bool), typeof(NotificationsPageViewModel));
        public bool IsLoggedIn
        {
            get => (bool)GetValue(IsLoggedInProperty);
            set => SetValue(IsLoggedInProperty, value);
        }
        public LoginResultType? LoginResult { get; private set; }
        public ICommand LogIn { get; }
        public ICommand LogOut { get; }

        protected readonly IIdentityService identityService;
        public BaseIdentityViewModel(IIdentityService identityService)
        {
            this.identityService = identityService;

            LogIn = new AsyncRelayCommand(OnLogIn);
            LogOut = new AsyncRelayCommand(OnLogOut);

            IsLoggedIn = identityService.IsLoggedIn();
            
            // Shell.Current.Navigation.PushModalAsync()

            RegisterEvents();
        }
        
        private async Task OnLogIn(CancellationToken cancellationToken = default)
        {
            LoginResult = await identityService.LoginAsync(cancellationToken).ConfigureAwait(false);
            MessagingCenter.Send<IIdentityViewModel, LoginResultType>(this, "LoggedIn", LoginResult ?? LoginResultType.UnknownError);
        }

        private async Task OnLogOut(CancellationToken cancellationToken = default)
        {
            await identityService.LogoutAsync().ConfigureAwait(false);
        }

        private void RegisterEvents()
        {
            identityService.LoggedIn += OnLoggedIn;
            identityService.LoggedOut += OnLoggedOut;
        }

        private void UnregisterEvents()
        {
            identityService.LoggedIn -= OnLoggedIn;
            identityService.LoggedOut -= OnLoggedOut;
        }
        protected virtual void OnLoggedIn(object? sender, EventArgs e)
        {
            IsLoggedIn = true;
        }
        protected virtual void OnLoggedOut(object? sender, EventArgs e)
        {
            IsLoggedIn = false;
            //Shell.Current?.Navigation.PushModalAsync(new LoginPage());
        }

        public virtual void Dispose()
        {
            UnregisterEvents();
        }
    }
}
