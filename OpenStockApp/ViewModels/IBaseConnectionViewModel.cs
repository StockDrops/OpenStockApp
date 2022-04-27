using Microsoft.Toolkit.Mvvm.Input;
using System.Windows.Input;

namespace OpenStockApp.ViewModels
{
    public interface IBaseConnectionViewModel
    {
        ICommand ConnectCommand { get; }
        bool IsConnected { get; }
        bool IsLoggedIn { get; set; }
        ICommand LogIn { get; }

        void Dispose();
        void OnClosed(object? sender, Exception? exception);
        Task OnConnect(CancellationToken cancellationToken = default);
        void OnConnected(object? sender, EventArgs eventArgs);
        void OnReconnected(object? sender, EventArgs eventArgs);
    }
}