using Microsoft.Extensions.Logging;

using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using OpenStockApp.Core.Contracts.Services.Users;
using OpenStockApp.Models;
using OpenStockApp.Models.Users;

namespace OpenStockApp.ViewModels.Settings
{
    public class UserSettingsViewModel : ObservableObject
    {
        public ObservableUser User { get; private set; }
        public RefreshButtonState RefreshUserButtonState { get; private set; }
        public RefreshButtonState LogInOutButtonState { get; private set; }
        public AsyncRelayCommand RefreshUserCommandAsync { get; set; }
        public AsyncRelayCommand LogInOutUserCommandAsync { get; set; }
        public AsyncRelayCommand GetUserCommand { get; private set; }

        //private readonly IUserDataService userDataService;
        //private readonly IUserService userService;
        private readonly LegacyApi.Services.LegacyApiService legacyApiService;
        private readonly IIdentityService identityService;
        private readonly ILogger logger;
        public UserSettingsViewModel(
            LegacyApi.Services.LegacyApiService legacyApiService,
            IIdentityService identityService,
            ILogger<UserSettingsViewModel> logger)
        {
            RefreshUserButtonState = new RefreshButtonState();
            LogInOutButtonState = new RefreshButtonState();
            User = new ObservableUser();

            this.identityService = identityService;
            this.logger = logger;
            this.legacyApiService = legacyApiService;

            GetUserCommand = new AsyncRelayCommand(GetUser);
            RefreshUserCommandAsync = new AsyncRelayCommand(RefreshUser);
            LogInOutUserCommandAsync = new AsyncRelayCommand(LogInOutUserAsync);
            RegisterServices();

            if (identityService.IsLoggedIn())
            {
                SetButtonsToLoggedInState();
            }
            else
            {
                SetButtonsToLoggedOutState();
            }
        }
        private void RegisterServices()
        {
            //userDataService.UserDataUpdated += OnUserDataUpdated;
            identityService.LoggedIn += OnLoggedIn;
            identityService.LoggedOut += OnLoggedOut;
        }
        private void OnLoggedIn(object? sender, EventArgs e)
        {
            SetButtonsToLoggedInState();
        }
        private void OnLoggedOut(object? sender, EventArgs e)
        {
            SetButtonsToLoggedOutState();
            
        }
        private void SetButtonsToLoggedInState()
        {
            //sets the button back to it's RefreshUser state
            RefreshUserButtonState.ButtonText = Resources.Strings.Resources.SettingsUserRefreshButton;
            RefreshUserButtonState.IsVisible = true;
            RefreshUserButtonState.IsEnabled = true;
            //Set log out button
            LogInOutButtonState.ButtonText = Resources.Strings.Resources.SettingsPageLogOutButtonText;
            LogInOutButtonState.IsVisible = true;
            LogInOutButtonState.IsEnabled = true;
        }
        private void SetButtonsToLoggedOutState()
        {
            if (User != null)
            {
                User.Name = Resources.Strings.Resources.SettingsDefaultUsername;
                User.SubscriptionLevel = "";
                User.UserName = "";
            }
            //sets the button back to it's RefreshUser state
            RefreshUserButtonState.ButtonText = Resources.Strings.Resources.SettingsUserRefreshButton;
            RefreshUserButtonState.IsVisible = false;
            RefreshUserButtonState.IsEnabled = false;
            //Set log out button
            LogInOutButtonState.ButtonText = Resources.Strings.Resources.SettingsPageLogInButtonText;
            LogInOutButtonState.IsVisible = true;
            LogInOutButtonState.IsEnabled = true;
        }

        //private void OnUserDataUpdated(object sender, UserDataUpdatedEventArgs userData)
        //{
        //    User?.SetUser(userData.BaseUserData);
        //}
        private async Task GetUser()
        {
            if (identityService.IsLoggedIn())
            {
                var user = await identityService.GetUserDefaultScopesAsync();
                var subscriptionLegacy = await legacyApiService.GetSubscriptionLevel();
                if(user != null)
                {
                    User.SetUser(user);
                }
                if(subscriptionLegacy != null)
                {
                    User.SubscriptionLevel = subscriptionLegacy.Name;
                }
            }
            else
            {
                SetButtonsToLoggedOutState();
            }

        }
        private async Task LogInOutUserAsync()
        {
            if (identityService.IsLoggedIn())
            {
                await identityService.LogoutAsync();
            }
            else
            {
//#if ANDROID
//                Application.Current.Dispatcher.BeginInvokeOnMainThread(async () =>
//                {
//                    var loginResult = await identityService.LoginAsync(null);
//                }
//                );
//#else
                var loginResult = await identityService.LoginAsync();
                switch (loginResult)
                {
                    case Core.Maui.Models.Users.LoginResultType.Success:
                        await GetUser();
                        break;
                    case Core.Maui.Models.Users.LoginResultType.NoNetworkAvailable:
                        break;
                    case Core.Maui.Models.Users.LoginResultType.TimedOut:
                        break;
                    case Core.Maui.Models.Users.LoginResultType.UnknownError:
                        break;
                }
//#endif
            }
        }
        private async Task RefreshUser()
        {
            try
            {
                if (identityService.IsLoggedIn())
                {
                    RefreshUserButtonState.ButtonText = OpenStockApp.Resources.Strings.Resources.SettingsUserRefreshing;
                    RefreshUserButtonState.IsEnabled = false;
                    //await userDataService.UpdateUserAsync(userInitiated: true);
                    await RefreshUserButtonState.TimeoutButton(OpenStockApp.Resources.Strings.Resources.SettingsUserRefreshingInTimeout, OpenStockApp.Resources.Strings.Resources.SettingsUserRefreshButton, 5000);
                }
            }
            catch (Exception e)
            {
                logger?.LogError(e, "");
            }

        }
    }
}
