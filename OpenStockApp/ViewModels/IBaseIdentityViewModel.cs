using System.Windows.Input;

namespace OpenStockApp.ViewModels
{
    public interface IIdentityViewModel
    {
        bool IsLoggedIn { get; set; }
        ICommand LogIn { get; }
        ICommand LogOut { get; }
    }
}