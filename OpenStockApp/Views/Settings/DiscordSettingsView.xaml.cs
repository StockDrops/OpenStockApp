using Microsoft.Maui.Controls.Xaml;
using OpenStockApp.Services;
using OpenStockApp.ViewModels.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenStockApp.Views.Settings;

[XamlCompilation(XamlCompilationOptions.Compile)]
public partial class DiscordSettingsView : ContentView
{
    //private Command test;
    //public Command SaveWebhookCommand
    //{
    //    get { return test; }
    //    set
    //    {
    //        test = value;
    //        OnPropertyChanged("SaveWebhookCommand");
    //    }
    //}
    public DiscordSettingsView()
    {
        BindingContext = ServiceLocator.GetService<DiscordSettingsViewModel>();
        InitializeComponent();
    }
}
