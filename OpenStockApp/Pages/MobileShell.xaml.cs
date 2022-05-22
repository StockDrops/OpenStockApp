using OpenStockApp.Services;
using OpenStockApp.ViewModels;

namespace OpenStockApp.Pages
{
    public partial class MobileShell : Shell
    {
        public MobileShell()
        {
            InitializeComponent();

            
            var shellModel = ServiceLocator.GetRequiredService<ShellViewModel>();
            BindingContext = shellModel;
            //NavigatedTo += shellModel.OnAppearing;
        }
    }
}
