using CommunityToolkit.Maui.Behaviors;
using Microsoft.Maui.Controls.Xaml;
using OpenStockApp.Services;
using OpenStockApp.ViewModels.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenStockApp.Views.Settings;

//[XamlCompilation(XamlCompilationOptions.Compile)]
public partial class UserSettingsView : ContentView
{
    public UserSettingsView() //UserSettingsViewModel viewModel
    {
        
        InitializeComponent();
        BindingContext = ServiceLocator.GetRequiredService<UserSettingsViewModel>();
    }
}
