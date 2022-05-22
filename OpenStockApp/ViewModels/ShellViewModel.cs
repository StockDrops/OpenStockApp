using OpenStockApp.Models;
using OpenStockApp.Pages.Alerts;
using OpenStockApp.Pages.Settings;
using OpenStockApp.Pages;
using Microsoft.Extensions.Logging;

namespace OpenStockApp.ViewModels;
public class ShellViewModel : BaseIdentityViewModel
{
    public AppSection Notifications { get; set; }
    public AppSection AlertSettings { get; set; }
    public AppSection Settings { get; set; }
    public AppSection PersonalizationSettings { get; set; }
    public AppSection IntegrationSettings { get; set; }
    public AppSection AboutSettings { get; set; }

    private readonly LoginPage loginPage;
    private readonly ILogger<ShellViewModel> logger;
    public ShellViewModel(Core.Contracts.Services.Users.IIdentityService identityService,
                          LoginPage loginPage,
                          ILogger<ShellViewModel> logger) : base(identityService)
    {
        this.loginPage = loginPage;
        this.logger = logger;

        Notifications = new AppSection(Resources.Strings.Resources.MainPageTitle,
        "app_indicator.png",
        "app_indicator_dark.png",
        typeof(NotificationsPage));


        AlertSettings = new AppSection(Resources.Strings.Resources.ShellAddNotifications,
        "bell.png",
        "bell_dark.png",
        typeof(AlertSettingsPage));

        Settings = new AppSection(Resources.Strings.Resources.ShellSettingsPage,
            "gear.png",
            "gear_dark.png"
            );
        
        IntegrationSettings = new AppSection
        {
            Title = Resources.Strings.Resources.SettingsIntegrationsTitle,
            TargetType = typeof(IntegrationsSettingsPage)
        };
        AboutSettings = new AppSection
        {
            Title = Resources.Strings.Resources.SettingsAboutPageTitle,
            TargetType = typeof(AboutSettingsPage)
        };
        PersonalizationSettings = new AppSection
        {
            Title = Resources.Strings.Resources.SettingsPagePersonalizationTitle,

        };
        //if(!IsLoggedIn)
        //    Shell.Current.Navigation.PushModalAsync(loginPage);
    }

    protected override async void OnLoggedOut(object? sender, EventArgs e)
    {
        base.OnLoggedOut(sender, e);
        try
        {
            await Shell.Current.Navigation.PushModalAsync(loginPage).ConfigureAwait(false);
        }
        catch(Exception ex)
        {
            logger.LogCritical(ex, "");
        }
        
      
    }
}
