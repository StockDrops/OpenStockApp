using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenStockApp.ViewModels.Settings
{
    public class PersonalizationViewModel : BindableBaseViewModel
    {
        public IUserDataViewModel UserDataViewModel { get; set; }
        public PersonalizationViewModel(IUserDataViewModel userDataViewModel)
        {
            UserDataViewModel = userDataViewModel;
        }
    }
}
