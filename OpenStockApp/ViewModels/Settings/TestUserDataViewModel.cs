using Microsoft.Maui.Dispatching;
using Microsoft.Toolkit.Mvvm.Input;
using OpenStockApp.Models;
using OpenStockApp.Models.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenStockApp.ViewModels.Settings
{
    internal class TestUserDataViewModel : IUserDataViewModel
    {
        public ButtonState LogInOutButtonState { get; set; }

        public AsyncRelayCommand LogInOutUserCommandAsync { get; set; }

        public ButtonState RefreshUserButtonState { get; set; }

        public AsyncRelayCommand RefreshUserCommandAsync { get; set; }

        public ObservableUser User { get; private set; }

        public TestUserDataViewModel()
        {
            User = new ObservableUser();
            LogInOutButtonState = new ButtonState
            {
                ButtonText = "Log out",
                IsEnabled = true,
                IsVisible = true
            };
            RefreshUserButtonState = new ButtonState
            {
                ButtonText = "Refresh User",
                IsEnabled = true,
                IsVisible = true
            };

            LogInOutUserCommandAsync = new AsyncRelayCommand(() => Task.CompletedTask);
            RefreshUserCommandAsync = new AsyncRelayCommand(async () =>
            {
                User.SetUser(new Core.Models.Users.User { DisplayName = "Test Display Name", UserPrincipalName = "Test Principal Name" });
                await Task.Delay(1000);
                User.SetUser(new Core.Models.Users.User { DisplayName = "Test Display Name 2", UserPrincipalName = "Test Principal Name 2" });
                
            });
        }
    }
}
