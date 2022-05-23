using Microsoft.Maui.Controls.Xaml;
using OpenStockApp.ViewModels.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenStockApp.Views.Settings;

//[XamlCompilation(XamlCompilationOptions.Compile)]
public partial class EmailSettingsView : ContentView
{
    public EmailSettingsView(EmailSettingsViewModel model)
    {
        BindingContext = model;
        //_ = model.InitializeAsync();
        InitializeComponent();

    }
}
