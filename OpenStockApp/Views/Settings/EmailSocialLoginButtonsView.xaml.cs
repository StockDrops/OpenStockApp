using Microsoft.Maui.Controls.Xaml;
using OpenStockApp.Services;
using OpenStockApp.ViewModels.Settings.Social;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace OpenStockApp.Views.Settings;

[XamlCompilation(XamlCompilationOptions.Compile)]
public partial class EmailSocialLoginButtonsView : ContentView
{
    public EmailSocialLoginButtonsView()
    {
        var viewModel = ServiceLocator.GetService<EmailSocialLoginButtonsViewModel>();
        BindingContext = viewModel;
        
        //_ = viewModel.InitializeAsync();
        InitializeComponent();
    }
}
