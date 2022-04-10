using OpenStockApp.Models;
using OpenStockApp.Pages.Alerts;
using OpenStockApp.Pages.Settings;

namespace OpenStockApp.ViewModels;
public class ShellViewModel
{
    public AppSection Notifications { get; set; }
    public AppSection AlertSettings { get; set; }
    public AppSection Settings { get; set; }
    public AppSection PersonalizationSettings { get; set; }
    public AppSection IntegrationSettings { get; set; }
    public AppSection AboutSettings { get; set; }
    public ShellViewModel()
    {
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
    }
}
