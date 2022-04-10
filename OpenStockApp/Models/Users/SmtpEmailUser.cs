using OpenStockApp.Email.Models.Email;

namespace OpenStockApp.Core.Maui.Models.Users;

public class SmtpEmailUser : BindableObject
{
    #region BindableProperties
    public static readonly BindableProperty IsFilledProperty =
        BindableProperty.Create(nameof(IsFilled), typeof(bool), typeof(SmtpEmailUser), false);
    public static readonly BindableProperty NameProperty =
        BindableProperty.Create(nameof(Name), typeof(string), typeof(SmtpEmailUser), string.Empty);
    public static readonly BindableProperty EmailAddressProperty =
        BindableProperty.Create(nameof(EmailAddress), typeof(string), typeof(SmtpEmailUser), string.Empty);
    public static readonly BindableProperty UsernameProperty =
        BindableProperty.Create(nameof(Username), typeof(string), typeof(SmtpEmailUser), string.Empty);
    public static readonly BindableProperty PasswordProperty =
        BindableProperty.Create(nameof(Password), typeof(string), typeof(SmtpEmailUser), string.Empty);
    public static readonly BindableProperty HostProperty =
        BindableProperty.Create(nameof(Host), typeof(string), typeof(SmtpEmailUser), string.Empty);
    public static readonly BindableProperty SmtpPortProperty =
        BindableProperty.Create(nameof(SmtpPort), typeof(int), typeof(SmtpEmailUser), 25);
    public static readonly BindableProperty UseSSLProperty =
        BindableProperty.Create(nameof(UseSSL), typeof(bool), typeof(SmtpEmailUser), true);
    public static readonly BindableProperty EnabledProperty =
        BindableProperty.Create(nameof(Enabled), typeof(bool), typeof(SmtpEmailUser), false);
    public static readonly BindableProperty EmailUserTypeProperty =
        BindableProperty.Create(nameof(EmailUserType), typeof(EmailUserType), typeof(SmtpEmailUser), EmailUserType.Sender);
    #endregion
    #region Properties

    public string? Name
    {
        get => (string)GetValue(NameProperty);
        set => SetValue(NameProperty, value);
    }
    public string? EmailAddress
    {
        get => (string)GetValue(EmailAddressProperty);
        set => SetValue(EmailAddressProperty, value);
    }
    public string? Username
    {
        get => (string)GetValue(UsernameProperty);
        set => SetValue(UsernameProperty, value);
    }
    public string? Password
    {
        get => (string)GetValue(PasswordProperty);
        set => SetValue(PasswordProperty, value);
    }
    public string? Host
    {
        get => (string)GetValue(HostProperty);
        set => SetValue(HostProperty, value);
    }
    public int SmtpPort
    {
        get => (int)GetValue(SmtpPortProperty);
        set => SetValue(SmtpPortProperty, value);
    }
    public bool UseSSL
    {
        get => (bool)GetValue(UseSSLProperty);
        set => SetValue(UseSSLProperty, value);
    }
    public bool Enabled
    {
        get => (bool)GetValue(EnabledProperty);
        set => SetValue(EnabledProperty, value);
    }
    public EmailUserType EmailUserType
    {
        get => (EmailUserType)GetValue(EmailUserTypeProperty);
        set => SetValue(EmailUserTypeProperty, value);
    }
    public bool IsFilled
    {
        get; set;
    }

    #endregion
    #region Constructors
    public SmtpEmailUser() : base() { }
    public SmtpEmailUser(EmailUser emailUser) : base()
    {
        Name = emailUser.Name;
        EmailAddress = emailUser.EmailAddress;
        Username = emailUser.Username;
        Password = emailUser.Password;
        Host = emailUser.Host;
        SmtpPort = emailUser.SmtpPort;
        UseSSL = emailUser.UseSSL;
        Enabled = emailUser.Enabled;
        EmailUserType = emailUser.EmailUserType;
    }
    #endregion
    #region Helpers
    public void CopyTo(ref EmailUser originalUser)
    {
        originalUser.Name = Name;
        originalUser.EmailAddress = EmailAddress;
        originalUser.Username = Username;
        originalUser.Password = Password;
        originalUser.Host = Host;
        originalUser.SmtpPort = SmtpPort;
        originalUser.UseSSL = UseSSL;
        originalUser.Enabled = Enabled;
        originalUser.EmailUserType = EmailUserType;
    }
    #endregion
}
