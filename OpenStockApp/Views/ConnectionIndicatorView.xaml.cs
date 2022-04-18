using System.Windows.Input;

namespace OpenStockApp;

[XamlCompilation(XamlCompilationOptions.Compile)]
public partial class ConnectionIndicatorView : ContentView
{
	public static readonly BindableProperty IsConnectedProperty = BindableProperty.Create(nameof(IsConnected), typeof(bool), typeof(ConnectionIndicatorView), false);

	public bool IsConnected
    {
		get => (bool)GetValue(IsConnectedProperty);
		set => SetValue(IsConnectedProperty, value);
    }

	public static readonly BindableProperty ConnectCommandProperty = BindableProperty.Create(nameof(ConnectCommand), typeof(ICommand), typeof(ConnectionIndicatorView), default);

	public ICommand ConnectCommand
    {
		get => (ICommand)GetValue(ConnectCommandProperty);
		set => SetValue(ConnectCommandProperty, value);
    }

	public ConnectionIndicatorView()
	{
		InitializeComponent();
		//BindingContext = this;
	}
}