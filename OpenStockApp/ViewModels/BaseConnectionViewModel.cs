using Microsoft.Toolkit.Mvvm.Input;
using OpenStockApp.Core.Contracts.Services.Hubs;
using System.Windows.Input;

namespace OpenStockApp.ViewModels
{
    public class BaseConnectionViewModel : BaseViewModel
    {
        static BindableProperty IsConnectedProperty =
            BindableProperty.Create(nameof(IsConnected), typeof(bool), typeof(BaseViewModel), false);

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
            if(baseHubClient.State == Microsoft.AspNetCore.SignalR.Client.HubConnectionState.Connected)
            {
                IsConnected = true;
            }
            else
            {
                await baseHubClient.StartAsync(cancellationToken);
            }
        }

        public BaseConnectionViewModel(IBaseHubClient baseHubClient)
        {
            this.baseHubClient = baseHubClient;

            IsConnected = baseHubClient.State == Microsoft.AspNetCore.SignalR.Client.HubConnectionState.Connected;
            ConnectCommand = new AsyncRelayCommand(OnConnect);

            this.baseHubClient.Closed += OnClosed;
            this.baseHubClient.Reconnected += OnReconnected;
            this.baseHubClient.Connected += OnConnected;
        }
    }
}
