using Microsoft.Toolkit.Mvvm.Input;
using OpenStockApp.Core.Contracts.Services.Hubs;
using OpenStockApp.Core.Contracts.Services.Users;
using OpenStockApp.ViewModels.Notifications;
using System.Windows.Input;

namespace OpenStockApp.ViewModels
{
    public class BaseConnectionViewModel : BaseIdentityViewModel, IDisposable, IBaseConnectionViewModel
    {
        static BindableProperty IsConnectedProperty =
            BindableProperty.Create(nameof(IsConnected), typeof(bool), typeof(BindableBaseViewModel), false);

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

        public BaseConnectionViewModel(IBaseHubClient baseHubClient, IIdentityService identityService) : base(identityService)
        {
            this.baseHubClient = baseHubClient;

            IsConnected = baseHubClient.State == Microsoft.AspNetCore.SignalR.Client.HubConnectionState.Connected;
            ConnectCommand = new AsyncRelayCommand(OnConnect);

            RegisterEvents();
        }

        private void RegisterEvents()
        {
            baseHubClient.Closed += OnClosed;
            baseHubClient.Reconnected += OnReconnected;
            baseHubClient.Connected += OnConnected;
        }

        private void UnregisterEvents()
        {
            baseHubClient.Closed -= OnClosed;
            baseHubClient.Reconnected -= OnReconnected;
            baseHubClient.Connected -= OnConnected;

        }
      
        public override void Dispose()
        {
            base.Dispose();
            UnregisterEvents();
        }
    }
}
