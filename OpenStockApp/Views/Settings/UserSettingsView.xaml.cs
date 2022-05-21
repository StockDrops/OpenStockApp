using CommunityToolkit.Maui.Behaviors;
using Microsoft.Maui.Controls.Xaml;
using OpenStockApp.Models;
using OpenStockApp.Models.Users;
using OpenStockApp.Services;
using OpenStockApp.ViewModels.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace OpenStockApp.Views.Settings;

//[XamlCompilation(XamlCompilationOptions.Compile)]
public partial class UserSettingsView : ContentView
{
    public static readonly BindableProperty UserProperty = BindableProperty.Create(nameof(User), typeof(ObservableUser), typeof(UserSettingsView));
    public static readonly BindableProperty RefreshButtonStateProperty = BindableProperty.Create(nameof(RefreshButtonState), typeof(ButtonState), typeof(UserSettingsView));
    public static readonly BindableProperty LogInOutButtonStateProperty = BindableProperty.Create(nameof(LogInOutButtonState), typeof(ButtonState), typeof(UserSettingsView));
    public static readonly BindableProperty RefreshCommandProperty = BindableProperty.Create(nameof(RefreshCommand), typeof(ICommand), typeof(UserSettingsView));
    public static readonly BindableProperty LogInOutCommandProperty = BindableProperty.Create(nameof(LogInOutCommand), typeof(ICommand), typeof(UserSettingsView));
    public static readonly BindableProperty ImageProfileSourceProperty = BindableProperty.Create(nameof(ImageProfileSource), typeof(string), typeof(UserSettingsView));



    public ObservableUser User
    {
        get => (ObservableUser)GetValue(UserProperty);
        set => SetValue(UserProperty, value);
    }
    public string? ImageProfileSource
    {
        get => (string?)GetValue(ImageProfileSourceProperty);
        set => SetValue(ImageProfileSourceProperty, value);
    }
    public ButtonState? RefreshButtonState
    {
        get => (ButtonState?)GetValue(RefreshButtonStateProperty);
        set => SetValue(RefreshButtonStateProperty, value);
    }

    public ButtonState? LogInOutButtonState
    {
        get => (ButtonState?)GetValue(LogInOutButtonStateProperty);
        set => SetValue(LogInOutButtonStateProperty, value);
    }
    public ICommand? RefreshCommand
    {
        get => (ICommand?)GetValue(RefreshCommandProperty);
        set => SetValue(RefreshCommandProperty, value);
    }
    public ICommand? LogInOutCommand
    {
        get => (ICommand?)GetValue(LogInOutCommandProperty);
        set => SetValue(LogInOutCommandProperty, value);
    }

    public UserSettingsView() //UserSettingsViewModel viewModel
    {
        
        InitializeComponent();
    }
}
