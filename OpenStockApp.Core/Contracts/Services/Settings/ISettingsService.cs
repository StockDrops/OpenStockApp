namespace OpenStockApp.Core.Contracts.Services.Settings
{
    public interface ISettingsService
    {
        public bool SaveSettingString(string key, string value);
        public bool SaveSettingBool(string key, bool value);
        public bool SaveSettingDouble(string key, double value);
        public bool LoadSettingString(string key, out string? value);
        public bool LoadSettingBool(string key, out bool value);
        public bool LoadSettingDouble(string key, out double value);
    }
}
