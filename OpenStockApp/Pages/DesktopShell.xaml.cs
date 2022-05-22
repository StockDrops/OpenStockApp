using OpenStockApp.Services;
using OpenStockApp.ViewModels;

namespace OpenStockApp.Pages;

public partial class DesktopShell : Shell
{
    public DesktopShell()
    {
        InitializeComponent();
        var shellModel = ServiceLocator.GetRequiredService<ShellViewModel>();
        BindingContext = shellModel;
    }
}