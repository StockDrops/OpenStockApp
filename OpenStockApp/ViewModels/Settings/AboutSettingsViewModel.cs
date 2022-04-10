using Microsoft.Extensions.Options;
using OpenStockApp.Core.Contracts.Services.Hubs;
using OpenStockApp.Core.Contracts.Services.Users;
using OpenStockApp.Models;
using System.Reflection;

namespace OpenStockApp.ViewModels.Settings
{
    
    public class AboutSettingsViewModel : BaseViewModel
    {
        public string? PrivacyStatementUrl => _appConfig?.PrivacyStatement;
        public string? LicenseAgreementUrl => _appConfig?.LicenseAgreement;
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

        public Command RefreshConnectionIdCommand { get; set; }
        private void RefreshConnectionId()
        {
            ConnectionId = _notificationsHubClient.ConnectionId;
        }


        private readonly AppConfig _appConfig;
        private readonly INotificationsHubClient _notificationsHubClient;
        private readonly IIdentityService identityService;
        public AboutSettingsViewModel(INotificationsHubClient notificationsHubClient, IIdentityService identityService, IOptions<AppConfig> appConfig)
        {
            _appConfig = appConfig.Value;
            _notificationsHubClient = notificationsHubClient;
            RefreshConnectionIdCommand = new Command(() => RefreshConnectionId());
            this.identityService = identityService;

            UserId = identityService.GetUniqueId();

            identityService.LoggedIn += OnLoggedIn;
            identityService.LoggedOut += OnLoggedOut;
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
