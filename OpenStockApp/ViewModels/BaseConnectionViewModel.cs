using Microsoft.Toolkit.Mvvm.Input;
using OpenStockApp.Core.Contracts.Services.Hubs;
using OpenStockApp.Core.Contracts.Services.Users;
using OpenStockApp.ViewModels.Notifications;
using System.Windows.Input;

namespace OpenStockApp.ViewModels
{
    public class BaseConnectionViewModel : BaseViewModel, IDisposable, IBaseConnectionViewModel
    {
        static BindableProperty IsConnectedProperty =
            BindableProperty.Create(nameof(IsConnected), typeof(bool), typeof(BaseViewModel), false);


        public BindableProperty IsLoggedInProperty = BindableProperty.Create(nameof(IsLoggedIn), typeof(bool), typeof(NotificationsPageViewModel));
        public bool IsLoggedIn
        {
            get => (bool)GetValue(IsLoggedInProperty);
            set => SetValue(IsLoggedInProperty, value);
        }

        private readonly IBaseHubClient baseHubClient;

        public bool IsConnected
        {
            get => (bool)GetValue(IsConnectedProperty);
            private set => SetValue(IsConnectedProperty, value);
        }

        public static readonly BindableProperty ConnectCommandProperty = BindableProperty.Create(nameof(ConnectCommand), typeof(ICommand), typeof(BaseConnectionViewModel), default);

        public ICommand ConnectCommand
        {
            get => (ICommand)GetValue(ConnectCommandProperty);
            private set => SetValue(ConnectCommandProperty, value);
        }

        public void OnClosed(object? sender, Exception? exception)
        {
            IsConnected = false;
        }

        public void OnReconnected(object? sender, EventArgs eventArgs)
        {
            IsConnected = true;
        }
        public void OnConnected(object? sender, EventArgs eventArgs)
        {
            IsConnected = true;
        }
        public async Task OnConnect(CancellationToken cancellationToken = default)
        {
            if (baseHubClient.State == Microsoft.AspNetCore.SignalR.Client.HubConnectionState.Connected)
            {
                IsConnected = true;
            }
            else
            {
                await baseHubClient.StartAsync(cancellationToken);
            }
        }
        public ICommand LogIn { get; }

        protected readonly IIdentityService identityService;
        public BaseConnectionViewModel(IBaseHubClient baseHubClient, IIdentityService identityService)
        {
            this.identityService = identityService;
            this.baseHubClient = baseHubClient;

            IsConnected = baseHubClient.State == Microsoft.AspNetCore.SignalR.Client.HubConnectionState.Connected;
            ConnectCommand = new AsyncRelayCommand(OnConnect);

            LogIn = new AsyncRelayCommand(OnLogIn);
            IsLoggedIn = identityService.IsLoggedIn();

            RegisterEvents();
        }

        private async Task OnLogIn(CancellationToken cancellationToken = default)
        {
            var result = await identityService.LoginAsync(cancellationToken).ConfigureAwait(false);
            MessagingCenter.Send(this, "LoggedIn", result);
        }

        private void RegisterEvents()
        {
            baseHubClient.Closed += OnClosed;
            baseHubClient.Reconnected += OnReconnected;
            baseHubClient.Connected += OnConnected;

            identityService.LoggedIn += OnLoggedIn;
            identityService.LoggedOut += OnLoggedOut;
        }

        private void UnregisterEvents()
        {
            baseHubClient.Closed -= OnClosed;
            baseHubClient.Reconnected -= OnReconnected;
            baseHubClient.Connected -= OnConnected;

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
        }

        public void Dispose()
        {
            UnregisterEvents();
        }
    }
}
