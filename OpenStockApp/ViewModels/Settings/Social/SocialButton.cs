using Microsoft.Toolkit.Mvvm.Input;
using OpenStockApp.Core.Email.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenStockApp.ViewModels.Settings.Social
{
    public class SocialButton : BindableObject
    {
        public readonly BindableProperty NameProperty =
            BindableProperty.Create(nameof(Name), typeof(string), typeof(SocialButton), string.Empty);

        public static readonly BindableProperty IsLoggedInProperty =
        BindableProperty.Create(nameof(IsLoggedIn), typeof(bool), typeof(SocialButton), true, propertyChanged: OnIsLoggedInPropertyChanged);
        public static readonly BindableProperty IsNotLoggedInProperty =
            BindableProperty.Create(nameof(IsNotLoggedIn2), typeof(bool), typeof(SocialButton), true);
        public bool IsNotLoggedIn2
        {
            get => (bool)GetValue(IsNotLoggedInProperty);
            set => SetValue(IsNotLoggedInProperty, value);
        }
        public bool IsLoggedIn
        {
            get => (bool)GetValue(IsLoggedInProperty);
            set => SetValue(IsLoggedInProperty, value);
        }
        public string Name
        {
            get => (string)GetValue(NameProperty);
            set => SetValue(NameProperty, value);
        }
        public static void OnIsLoggedInPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        => ((SocialButton)bindable).OnIsLoggedInPropertyChanged();
        public void OnIsLoggedInPropertyChanged()
        {
            IsNotLoggedIn2 = !IsLoggedIn;
        }

        public AsyncRelayCommand LoginCommand { get; private set; }

        private readonly IEmailService emailService;
        public SocialButton(IEmailService emailService)
        {
            this.emailService = emailService;
            Name = emailService.ServiceName;
            Initialize();
            emailService.IdentityService.LoggedIn += OnLoggedIn;
            emailService.IdentityService.LoggedOut += OnLoggedOut;

            LoginCommand = new AsyncRelayCommand(async token => await emailService.IdentityService.LoginAsync(token));
        }
        private void Initialize()
        {
            IsLoggedIn = emailService.IdentityService.IsLoggedIn();
        }
        private void OnLoggedIn(object? sender, EventArgs e)
        {
            //before seting this to true let's change it to the opposite;
            IsLoggedIn = false;
            IsLoggedIn = true;
        }
        private void OnLoggedOut(object? sender, EventArgs e)
        {
            IsLoggedIn = true; //like this we can force the event to fire.
            IsLoggedIn = false;
        }
    }
}
