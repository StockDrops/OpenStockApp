using Microsoft.Toolkit.Mvvm.ComponentModel;
using OpenStockApp.Core.Maui.Services;

namespace OpenStockApp.ViewModels.Settings
{
    public class ThemeViewModel : ObservableObject
    {

        private AppTheme theme;
        public AppTheme Theme
        {
            get => theme;
            set
            {
                if (theme != value)
                {
                    ThemeSelectorService.SetTheme(value);
                }
                SetProperty(ref theme, value);
            }
        }
        
        public ThemeViewModel()
        {
            Theme = ThemeSelectorService.GetCurrentTheme();
        }
    }
}
