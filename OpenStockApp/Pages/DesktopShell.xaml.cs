using OpenStockApp.ViewModels;

namespace OpenStockApp.Pages;

public partial class DesktopShell : Shell
{
    public DesktopShell()
    {
        InitializeComponent();

        BindingContext = new ShellViewModel();
    }
}