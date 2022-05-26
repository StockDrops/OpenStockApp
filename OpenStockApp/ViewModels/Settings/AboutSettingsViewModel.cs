
using Microsoft.Extensions.Options;
using Microsoft.Toolkit.Mvvm.Input;
using OpenStockApp.Core.Contracts.Services.Hubs;
using OpenStockApp.Core.Contracts.Services.Users;
using OpenStockApp.Models;
using OpenStockApp.Core.Contracts.Services.Settings;

#if ANDROID
using Firebase.Messaging;
using Android.Gms.Extensions;
#endif
using System.Reflection;

namespace OpenStockApp.ViewModels.Settings
{
    
    public class AboutSettingsViewModel : BindableBaseViewModel
    {
        public string? PrivacyStatementUrl => _appConfig?.PrivacyStatement;
        public string? LicenseAgreementUrl => _appConfig?.LicenseAgreement;
        public string? CurrentYear => DateTime.Now.Year.ToString();
        public string? UserPortalUrl => _appConfig?.UserPortal;
        public string? VersionDescription => Assembly.GetExecutingAssembly()?.GetName()?.Version?.ToString();

        public static readonly BindableProperty ConnectionIdProperty = BindableProperty.Create(nameof(ConnectionId), typeof(string), typeof(AboutSettingsViewModel));
        public static readonly BindableProperty UserIdProperty = BindableProperty.Create(nameof(UserId), typeof(string), typeof(AboutSettingsViewModel));

        public string? ConnectionId
        {
            get => (string?)GetValue(ConnectionIdProperty);
            set => SetValue(ConnectionIdProperty, value);
        }
        public string? UserId
        {
            get => (string?)GetValue(UserIdProperty);
            set => SetValue(UserIdProperty, value);
        }
        public static readonly BindableProperty FirebaseTokenProperty = BindableProperty.Create(nameof(FirebaseToken), typeof(string), typeof(AboutSettingsViewModel));
        public string? FirebaseToken
        {
            get => (string?)GetValue(FirebaseTokenProperty);
            set => SetValue(FirebaseTokenProperty, value);
        }
        public static readonly BindableProperty UserOptionsIdProperty = BindableProperty.Create(nameof(UserOptionsId), typeof(long), typeof(AboutSettingsViewModel));
        public long UserOptionsId
        {
            get => (long)GetValue(UserOptionsIdProperty);
            set => SetValue(UserOptionsIdProperty, value);
        }
        public Command RefreshUserOptionsIdCommand { get; }
        public Command RefreshConnectionIdCommand { get; set; }
        public AsyncRelayCommand RefreshTokenCommand { get; set; }
        private void RefreshConnectionId()
        {
            ConnectionId = _notificationsHubClient.ConnectionId;
        }
        private async Task RefreshFirebaseToken(CancellationToken cancellationToken = default)
        {
#if ANDROID
            FirebaseToken = (await FirebaseMessaging.Instance.GetToken().AsAsync<Java.Lang.String>()).ToString();
#endif
        }
        public AsyncRelayCommand ShareLog { get; set; }


        private readonly AppConfig _appConfig;
        private readonly INotificationsHubClient _notificationsHubClient;
        private readonly IIdentityService identityService;
        private readonly IUserOptionsService userOptionsService;
        public AboutSettingsViewModel(INotificationsHubClient notificationsHubClient, IIdentityService identityService, IOptions<AppConfig> appConfig, IUserOptionsService userOptionsService)
        {
            _appConfig = appConfig.Value;
            _notificationsHubClient = notificationsHubClient;
            RefreshConnectionIdCommand = new Command(() => RefreshConnectionId());
            RefreshTokenCommand = new AsyncRelayCommand(RefreshFirebaseToken);
            ShareLog = new AsyncRelayCommand(OnShareLog);
            this.identityService = identityService;
            this.userOptionsService = userOptionsService;
            UserId = identityService.GetUniqueId();
            UserOptionsId = userOptionsService.UserOptions?.Id ?? 0;
            identityService.LoggedIn += OnLoggedIn;
            identityService.LoggedOut += OnLoggedOut;

            RefreshUserOptionsIdCommand = new Command(() =>
            {
                UserOptionsId = this.userOptionsService.UserOptions?.Id ?? 0;
            });
        }

        public async Task OnShareLog(CancellationToken cancellationToken = default)
        {
            var path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), Paths.LogFile);
            await Share.RequestAsync(new ShareFileRequest
            {
                Title = Title,
                File = new ShareFile(path)
            });
        }

        private void OnLoggedIn(object? sender, EventArgs eventArgs)
        {
            if (identityService.IsLoggedIn())
            {
                UserId = identityService.GetUniqueId();
            }
        }
        private void OnLoggedOut(object? sender, EventArgs eventArgs)
        {
            UserId = string.Empty;
        }
    }
}
