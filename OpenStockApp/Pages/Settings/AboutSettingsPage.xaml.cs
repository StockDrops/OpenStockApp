using Microsoft.Maui.Controls.Xaml;
using OpenStockApp.ViewModels.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenStockApp.Pages.Settings;

//[XamlCompilation(XamlCompilationOptions.Compile)]
public partial class AboutSettingsPage : ContentPage
{
    public AboutSettingsPage(AboutSettingsViewModel aboutSettingsViewModel)
    {
        BindingContext = aboutSettingsViewModel;
        InitializeComponent();
    }
}
