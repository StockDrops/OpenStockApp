using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenStockApp.Core.Maui.Services
{
    public class ThemeSelectorService
    {

        public ThemeSelectorService()
        {
        }

        public static void InitializeTheme()
        {
            var theme = GetCurrentTheme();
            SetTheme(theme);
        }

        public static void SetTheme(AppTheme theme)
        {
            if(Application.Current != null)
            {
                switch (theme)
                {
                    case AppTheme.Dark:
                        Application.Current.UserAppTheme = AppTheme.Dark;
                        break;
                    case AppTheme.Light:
                        Application.Current.UserAppTheme = AppTheme.Light;
                        break;
                    default:
                        Application.Current.UserAppTheme = AppTheme.Unspecified;
                        break;
                }
            }
            Preferences.Set("Theme", theme.ToString());
            //App.Current.Properties["Theme"] = theme.ToString();
        }

        public static AppTheme GetCurrentTheme()
        {
            if (Preferences.ContainsKey("Theme"))
            {
                var themeName = Preferences.Get("Theme", "Dark");
                Enum.TryParse(themeName, out AppTheme theme);
                return theme;
            }

            return AppTheme.Unspecified;
        }
    }
}
