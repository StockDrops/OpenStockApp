using Microsoft.Extensions.Logging;
using MvvmHelpers.Commands;
using Microsoft.Toolkit.Mvvm.Input;
using OpenStockApp.Models;
using OpenStockApp.Core.Maui.Models.Users;
using OpenStockApp.Email.Models.Email;

/* Unmerged change from project 'OpenStockApp (net6.0-maccatalyst)'
Before:
using OpenStockApp.Email.Contracts;
After:
using OpenStockApp.Email.Contracts;
using OpenStockApp.ViewModels.Settings;
using StockDrops;
using StockDrops.Maui;
using StockDrops.Maui.ViewModels;
using StockDrops.Maui.ViewModels.Settings;
*/

/* Unmerged change from project 'OpenStockApp (net6.0-ios)'
Before:
using OpenStockApp.Email.Contracts;
After:
using OpenStockApp.Email.Contracts;
using OpenStockApp.ViewModels.Settings;
using StockDrops;
using StockDrops.Maui;
using StockDrops.Maui.ViewModels;
using StockDrops.Maui.ViewModels.Settings;
*/

/* Unmerged change from project 'OpenStockApp (net6.0-windows10.0.19041)'
Before:
using OpenStockApp.Email.Contracts;
After:
using OpenStockApp.Email.Contracts;
using OpenStockApp.ViewModels.Settings;
using StockDrops;
using StockDrops.Maui;
using StockDrops.Maui.ViewModels;
using StockDrops.Maui.ViewModels.Settings;
*/
using OpenStockApp.Core.Email.Contracts;

//#if ANDROID
//using StockDrops.Core.Platforms.Android.Google;
//#endif
namespace OpenStockApp.ViewModels.Settings;

public class EmailSettingsViewModel : BindableBaseViewModel
{
    public ConfirmationMessage EmailOAuthConfirmationMessage { get; set; } = new ConfirmationMessage { IsVisible = false, Message = "", IsSuccess = false };
    private EmailUser emailUser;

    public static readonly BindableProperty IsLoggedInProperty =
        BindableProperty.Create(nameof(IsLoggedIn), typeof(bool), typeof(EmailSettingsViewModel), true, propertyChanged: OnIsLoggedInPropertyChanged);
    public static readonly BindableProperty IsNotLoggedInProperty =
        BindableProperty.Create(nameof(IsNotLoggedIn), typeof(bool), typeof(EmailSettingsViewModel), true);
    public bool IsNotLoggedIn
    {
        get => (bool)GetValue(IsNotLoggedInProperty);
        set => SetValue(IsNotLoggedInProperty, value);
    }
    public bool IsLoggedIn
    {
        get => (bool)GetValue(IsLoggedInProperty);
        set => SetValue(IsLoggedInProperty, value);
    }
    public static void OnIsLoggedInPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        => ((EmailSettingsViewModel)bindable).OnIsLoggedInPropertyChanged();

    void OnIsLoggedInPropertyChanged() => IsNotLoggedIn = !IsLoggedIn;

    public static readonly BindableProperty LoggedInUserProperty =
        BindableProperty.Create(nameof(LoggedInUser), typeof(string), typeof(EmailSettingsViewModel), string.Empty);
    public string LoggedInUser
    {
        get => (string)GetValue(LoggedInUserProperty);
        set => SetValue(LoggedInUserProperty, value);
    }
    /// <summary>
    /// Command to  log in and log out from GMAIL.
    /// </summary>
    public AsyncRelayCommand LogInOutGmailCommand { get; set; }
    /// <summary>
    /// Command to log in and log out from Hotmail/Microsoft.
    /// </summary>
    public AsyncRelayCommand LogInOutHotmailCommand { get; set; }
    /// <summary>
    /// Test the email service of choice here.
    /// </summary>
    public AsyncRelayCommand TestLoggedInEmailService { get; set; }
    /// <summary>
    /// Save the email from the SMTP settings.
    /// </summary>
    public Microsoft.Maui.Controls.Command SaveEmailCommand { get; set; }
    /// <summary>
    /// Backing field for the SMTP Email User inputs.
    /// </summary>
    public SmtpEmailUser SmtpEmailUser { get; set; }
    /// <summary>
    /// Log out command.
    /// </summary>
    public AsyncRelayCommand LogOutCommand { get; set; }

    private readonly ISmtpEmailService emailUserService;
    private readonly ILogger<EmailSettingsViewModel> logger;
    private readonly IEnumerable<IEmailService> emailServices; //TODO: make this into a list of email services.
    public EmailSettingsViewModel(ISmtpEmailService emailUserService,
        IEnumerable<IEmailService> emailServices,
        ILogger<EmailSettingsViewModel> logger)
    {
        this.emailUserService = emailUserService;
        this.emailServices = emailServices;
        //microsoftEmailService.Intialize();
        foreach (var emailService in emailServices)
        {
            emailService.IdentityService.LoggedIn += OnLoggedIn!;
            emailService.IdentityService.LoggedOut += OnLoggedOut!;
        }
        this.logger = logger;
        LoadEmailUser();
        SaveEmailCommand = new Microsoft.Maui.Controls.Command(SaveEmailUser);
        //this.LogInOutHotmailCommand = new AsyncRelayCommand(LogInOutHotmail);
        ////this.TestLoggedInEmailService = new AsyncRelayCommand(TestMicrosoftEmailService);
        //this.LogInOutGmailCommand = new AsyncRelayCommand(LogInOutGmail);
        //this.LogOutCommand = new AsyncRelayCommand(LogOutAsync);
    }
    private async void OnLoggedIn(object sender, EventArgs e)
    {
        //if (!IsLoggedIn)
        //    await InitializeAsync();
        IsLoggedIn = true;
    }
    private void OnLoggedOut(object sender, EventArgs e)
    {
        IsLoggedIn = false;
        LoggedInUser = "";
    }
    /// <summary>
    /// Run this method to send a test email to the logged in microsoft email account.
    /// </summary>
    /// <returns></returns>
    //private async Task TestMicrosoftEmailService(CancellationToken token)
    //{
    //    foreach (var emailService in emailServices)
    //    {
    //        if (emailService.IdentityProvider.IsLoggedIn())
    //        {
    //            try
    //            {
    //                await emailService.SendEmailToLoggedInUserAsync("Test From StockDrops v2", new Core.Models.Backend.Integrations.EmailBody
    //                {
    //                    BodyType = BodyType.Html,
    //                    Content = "Test"
    //                }, token);
    //                EmailOAuthConfirmationMessage.Message = "Email Sent.";
    //                EmailOAuthConfirmationMessage.IsVisible = true;
    //                EmailOAuthConfirmationMessage.IsSuccess = true;
    //            }
    //            catch (Exception e)
    //            {

    //            }
    //        }
    //    }
    //}
    //private async Task LogInOutGmail(CancellationToken token)
    //{
    //    var res = await googleIdentyService.LoginAsync(token);
    //    switch (res)
    //    {
    //        case LoginResultType.Success:
    //            //handle success
    //            break;
    //        case LoginResultType.CancelledByUser:
    //            //handle it
    //            break;
    //        default:
    //            break;
    //    }
    //}
    /// <summary>
    /// Use this to save the email to the database.
    /// </summary>
    private void SaveEmailUser()
    {
        if (emailUser != null)
        {
            SmtpEmailUser.CopyTo(ref emailUser);
            emailUserService.SaveEmailUser(emailUser);
        }
    }
    /// <summary>
    /// Load the email user from the database.
    /// </summary>
    private void LoadEmailUser()
    {
        emailUser = emailUserService.RetrieveEmailUser();
        if (emailUser != null)
        {
            SmtpEmailUser = new SmtpEmailUser(emailUser);
        }
        else
        {
            SmtpEmailUser = new SmtpEmailUser();
        }
    }
    //public async Task InitializeAsync()
    //{
    //    var loggedInUser = "";
    //    foreach (var emailService in emailServices)
    //    {
    //        if (emailService.IdentityProvider.IsLoggedIn())
    //        {
    //            var user = (await emailService.IdentityProvider.GetUserDefaultScopesAsync());
    //            loggedInUser += $"{user.Mail ?? user.UserPrincipalName}\n";
    //        }
    //    }
    //    if (!string.IsNullOrEmpty(loggedInUser))
    //    {
    //        LoggedInUser = "User logged in: " + loggedInUser;
    //        await Task.Delay(500);
    //        IsLoggedIn = true;
    //        IsNotLoggedIn = false;
    //    }
    //    else
    //    {
    //        await Task.Delay(500);
    //        IsLoggedIn = false;
    //        IsNotLoggedIn = true;
    //    }
    //}
    //private async Task LogOutAsync(CancellationToken token)
    //{
    //    IsLoggedIn = !IsLoggedIn;
    //    foreach (var emailService in emailServices)
    //    {
    //        if (emailService.IdentityProvider.IsLoggedIn())
    //        {
    //            await emailService.IdentityProvider.LogoutAsync();
    //            LoggedInUser = "";
    //            IsLoggedIn = false;
    //        }
    //    }
    //}
    private async Task LogInOutHotmail(CancellationToken token)
    {
        //var result = await microsoftEmailService.LoginAsync(token);
        //if(result == LoginResultType.Success)
        //{
        //    //show success.
        //    Console.WriteLine("Success!");
        //}
    }

}
