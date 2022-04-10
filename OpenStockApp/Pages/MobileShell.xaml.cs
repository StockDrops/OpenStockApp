using OpenStockApp.ViewModels;

namespace OpenStockApp.Pages
{
    public partial class MobileShell
    {
        public MobileShell()
        {
            InitializeComponent();

            BindingContext = new ShellViewModel();
        }
    }
}
