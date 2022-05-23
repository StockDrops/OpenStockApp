using Microsoft.Toolkit.Mvvm.Input;
using OpenStockApp.Models;
using OpenStockApp.Models.Users;

namespace OpenStockApp.ViewModels.Settings
{
    public interface IUserDataViewModel
    {
        ButtonState LogInOutButtonState { get; }
        AsyncRelayCommand LogInOutUserCommandAsync { get; set; }
        ButtonState RefreshUserButtonState { get; }
        AsyncRelayCommand RefreshUserCommandAsync { get; set; }
        ObservableUser User { get; }
    }
}