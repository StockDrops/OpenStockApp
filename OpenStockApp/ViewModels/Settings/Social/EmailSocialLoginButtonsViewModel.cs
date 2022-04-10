using OpenStockApp.Email.Contracts;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenStockApp.ViewModels.Settings.Social
{
    public class EmailSocialLoginButtonsViewModel
    {
        public ObservableCollection<SocialButton> SocialButtons { get; } = new();
        private readonly IEnumerable<IEmailService> emailServices;
        public EmailSocialLoginButtonsViewModel(IEnumerable<IEmailService> emailServices)
        {
            this.emailServices = emailServices;
            LoadButtons();
        }
        private void LoadButtons()
        {
            foreach (var emailService in emailServices)
            {
                SocialButtons.Add(new SocialButton(emailService));
            }
        }
    }
}
