using System.Windows.Input;

namespace OpenStockApp.Views;

public partial class LogInView : ContentView
{
    public static readonly BindableProperty IsLoggedInProperty = BindableProperty.Create(nameof(IsLoggedIn), typeof(bool), typeof(LogInView));
    public static readonly BindableProperty LogInCommandProperty = BindableProperty.Create(nameof(LogInCommand), typeof(ICommand), typeof(LogInView));
    public bool IsLoggedIn
    {
        get => (bool)GetValue(IsLoggedInProperty);
        set => SetValue(IsLoggedInProperty, value);
    }
    public ICommand? LogInCommand
    {
        get => (ICommand?)GetValue(LogInCommandProperty);
        set => SetValue(LogInCommandProperty, value);
    }
    public LogInView()
	{
		InitializeComponent();
	}
}