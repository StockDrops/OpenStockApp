using OpenStockApp.Core.Contracts.Services.Users;
using OpenStockApp.Pages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenStockApp.ViewModels.Users
{
    public class LoginViewModel : BaseIdentityViewModel, IIdentityViewModel
    {
        public LoginViewModel(IIdentityService identityService) : base(identityService)
        {

        }
        protected override async void OnLoggedIn(object? sender, EventArgs e)
        {
            base.OnLoggedIn(sender, e);
            System.Diagnostics.Debug.WriteLine("popping");
            await (Shell.Current?.Navigation?.PopModalAsync(true) ?? Task.CompletedTask).ConfigureAwait(false);
                           
        }

        protected override void OnLoggedOut(object? sender, EventArgs e)
        {
            base.OnLoggedOut(sender, e);
            
        }
    }
}
