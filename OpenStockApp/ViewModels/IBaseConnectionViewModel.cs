using Microsoft.Toolkit.Mvvm.Input;
using System.Windows.Input;

namespace OpenStockApp.ViewModels
{
    public interface IBaseConnectionViewModel : IBaseViewModel
    {
        public ICommand ConnectCommand { get; }
        public bool IsConnected { get; }
        public bool IsLoggedIn { get; set; }
        public ICommand LogIn { get; }

        public void Dispose();
        public void OnClosed(object? sender, Exception? exception);
        public Task OnConnect(CancellationToken cancellationToken = default);
        public void OnConnected(object? sender, EventArgs eventArgs);
        public void OnReconnected(object? sender, EventArgs eventArgs);
    }
}