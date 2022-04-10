using Microsoft.Extensions.Logging;
using OpenStockApp.Core.Contracts.Services.Settings;

namespace OpenStockApp.Core.Maui.Services.Settings;
public class SettingsService : ISettingsService
{
    private readonly ILogger<SettingsService> logger;
    public SettingsService(ILogger<SettingsService> logger)
    {
        this.logger = logger;
    }
    public bool SaveSettingBool(string key, bool value)
    {
        try
        {
            Preferences.Set(key, value);
            return true;
        }
        catch (Exception e)
        {
            logger?.LogError(e, "");
        }
        return false;

    }
    public bool SaveSettingDouble(string key, double value)
    {
        try
        {
            Preferences.Set(key, value);
            //App.Current.Properties[key] = value.ToString();
            //persistAndRestoreService.PersistData();
            return true;
        }
        catch (Exception e)
        {
            logger?.LogError(e, "");
        }
        return false;
    }
    public bool LoadSettingDouble(string key, out double value)
    {
        if (Preferences.ContainsKey(key))
        {
            try
            {
                value = Preferences.Get(key, -1.0);
                return true;
            }
            catch (InvalidCastException e)
            {
                logger?.LogError(e, "");
            }

        }
        value = default;
        return false;
    }

    public bool LoadSettingBool(string key, out bool value)
    {
        if (Preferences.ContainsKey(key))
        {
            try
            {
                value = Preferences.Get(key, false);
                return true;
            }
            catch (InvalidCastException e)
            {
                logger?.LogError(e, "");
            }
        }
        value = default;
        return false;
    }

    public bool SaveSettingString(string key, string value)
    {
        Preferences.Set(key, value);
        return true;
    }

    public bool LoadSettingString(string key, out string? value)
    {
        if (Preferences.ContainsKey(key))
        {
            try
            {
                value = Preferences.Get(key, string.Empty);
                return true;
            }
            catch (InvalidCastException e)
            {
                logger?.LogError(e, "");
            }
        }
        value = default;
        return false;
    }
}
