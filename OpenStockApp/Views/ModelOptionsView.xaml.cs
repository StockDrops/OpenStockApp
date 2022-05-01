using Microsoft.Toolkit.Mvvm.Input;
using OpenStockApi.Core.Models.Products;
using OpenStockApp.Core.Maui.Models;
using OpenStockApp.Extensions;
using OpenStockApp.Models.Users;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace OpenStockApp.Views;

public partial class ModelOptionsView : ContentView
{
    public static readonly BindableProperty ItemSourceProperty = BindableProperty.Create(nameof(ItemSource), typeof(ObservableCollection<GroupedObversableModelOptions>), typeof(ModelOptionsView), new ObservableCollection<GroupedObversableModelOptions>());
        //, propertyChanged:
        //(b, o, s) => OnListChanged(b, o, s));

    public ObservableCollection<GroupedObversableModelOptions> ItemSource
    {
        get => (ObservableCollection<GroupedObversableModelOptions>)GetValue(ItemSourceProperty);
        set => SetValue(ItemSourceProperty, value);

    }



    public static readonly BindableProperty NotificationActionsProperty = BindableProperty.Create(nameof(NotificationActions),
        typeof(ObservableCollection<DisplayedNotificationActions>), typeof(ModelOptionsView), new ObservableCollection<DisplayedNotificationActions>()); // , propertyChanged: (b, o, n) => OnNotificationActionsChanged(b, o, n));
	public ObservableCollection<DisplayedNotificationActions> NotificationActions
    {
		get => (ObservableCollection<DisplayedNotificationActions>)GetValue(NotificationActionsProperty);
		set {
            
            SetValue(NotificationActionsProperty, value);
        }
    }

    public static BindableProperty IsBusyProperty = BindableProperty.Create(nameof(IsBusy), typeof(bool), typeof(ModelOptionsView), defaultValue: false);

    public bool IsBusy
    {
        get
        {
            return (bool)GetValue(IsBusyProperty);
        }
        set
        {
            SetValue(IsBusyProperty, value);
        }
    }
    public static readonly BindableProperty IsLoggedInProperty = BindableProperty.Create(nameof(IsLoggedIn), typeof(bool), typeof(ModelOptionsView), defaultValue: false);
    public bool IsLoggedIn
    {
        get
        {
            return (bool)GetValue(IsLoggedInProperty);
        }
        set
        {
            SetValue(IsLoggedInProperty, value);
        }
    }
    public static readonly BindableProperty LogInCommandProperty = BindableProperty.Create(nameof(LogInCommand), typeof(ICommand), typeof(ModelOptionsView), default);

    public ICommand LogInCommand
    {
        get => (ICommand)GetValue(LogInCommandProperty);
        set => SetValue(LogInCommandProperty, value);
    }

    public static readonly BindableProperty ReloadSourceCommandProperty = BindableProperty.Create(nameof(ReloadSourceCommand), typeof(ICommand), typeof(ModelOptionsView), default);
    public ICommand ReloadSourceCommand
    {
        get => (ICommand)GetValue(ReloadSourceCommandProperty);
        set => SetValue(ReloadSourceCommandProperty, value);
    }
    //public static readonly BindableProperty SaveModelOptionsCommandProperty = BindableProperty.Create(nameof(SaveModelOptionsCommand), typeof(ICommand), typeof(ModelOptionsView), default);
    //public ICommand SaveModelOptionsCommand
    //{
    //    get => (ICommand)GetValue(SaveModelOptionsCommandProperty);
    //    set => SetValue(SaveModelOptionsCommandProperty, value);
    //}

    public static readonly BindableProperty PerformSearchCommandProperty = BindableProperty.Create(nameof(PerformSearchCommand), typeof(Command<string>), typeof(ModelOptionsView), default);

    public Command<string> PerformSearchCommand
    {
        get => (Command<string>)GetValue(PerformSearchCommandProperty);
        set => SetValue(PerformSearchCommandProperty, value);
    }

    public static readonly BindableProperty HelpTextProperty = BindableProperty.Create(nameof(HelpText), typeof(string), typeof(ModelOptionsView), string.Empty);

    public string? HelpText
    {
        get => (string?)GetValue(HelpTextProperty);
        set => SetValue(HelpTextProperty, value);
    }

    public static readonly BindableProperty HelpTextTitleProperty = BindableProperty.Create(nameof(HelpTextTitle), typeof(string), typeof(ModelOptionsView), string.Empty);

    public string? HelpTextTitle
    {
        get => (string?)GetValue(HelpTextTitleProperty);
        set => SetValue(HelpTextTitleProperty, value);
    }


    public static readonly BindableProperty ProductsProperty = BindableProperty.Create(nameof(Products), typeof(ObservableCollection<Product>), typeof(ModelOptionsView), new ObservableCollection<Product>());

    public ObservableCollection<Product> Products
    {
        get => (ObservableCollection<Product>)GetValue(ProductsProperty);
        set => SetValue(ProductsProperty, value);
    }

    public static readonly BindableProperty SelectedProductProperty = BindableProperty.Create(nameof(SelectedProduct), typeof(Product), typeof(ModelOptionsView), null);
    public Product? SelectedProduct
    {
        get => (Product?)GetValue(SelectedProductProperty);
        set => SetValue(SelectedProductProperty, value);
    }

    public static readonly BindableProperty SelectedProductCommandProperty = BindableProperty.Create(nameof(SelectedProductCommand), typeof(ICommand), typeof(ModelOptionsView), default);

    public ICommand SelectedProductCommand
    {
        get => (ICommand)GetValue(SelectedProductCommandProperty);
        set => SetValue(SelectedProductCommandProperty, value);
    }

    public static readonly BindableProperty ProductPickerTitleProperty = BindableProperty.Create(nameof(ProductPickerTitle), typeof(string), typeof(ModelOptionsView), string.Empty);

    public string ProductPickerTitle 
    { 
        get => (string)GetValue(ProductPickerTitleProperty); 
        set => SetValue(ProductPickerTitleProperty, value); 
    }

    public static readonly BindableProperty SaveCommandProperty = BindableProperty.Create(nameof(SaveCommand), typeof(ICommand), typeof(ModelOptionsView), default);

    public ICommand SaveCommand
    {
        get => (ICommand)GetValue(SaveCommandProperty);
        set => SetValue(SaveCommandProperty, value);
    }

    public Command SelectCommand { get; }
    public Command DeselectCommand { get; }

    public ICommand HelpCommand { get; }
    public ModelOptionsView()
	{
        //BindingContext = this;

        PerformSearchCommand = new Command<string>(
            (query) =>
            {
                OnPerformSearch(query);
            });
        SelectCommand = new Command(() => OnSelectCommand());
        DeselectCommand = new Command(() => OnDeselectCommand());
        HelpCommand = new AsyncRelayCommand(OnDisplayHelpAsync);
        InitializeComponent();
    }

    private void OnSelectCommand()
    {
        SelectDeselectItems(true);
    }
    private void SelectDeselectItems(bool select = true)
    {
        IsBusy = true;
        foreach (var group in ItemSource)
        {
            foreach (var item in group)
            {
                item.ModelOptions.IsEnabled = select;
            }
        }
        IsBusy = false;
    }
    private async Task OnDisplayHelpAsync(CancellationToken cancellationToken = default)
    {
        await (Application.Current?.MainPage?.DisplayAlert(HelpTextTitle ?? "Help", HelpText, "Ok") ?? Task.CompletedTask);
    }
    private void OnDeselectCommand()
    {
        SelectDeselectItems(false);
    }

    private static void OnListChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if(oldValue is ObservableCollection<GroupedObversableModelOptions> list && newValue is ObservableCollection<GroupedObversableModelOptions> newList)
        {
            var control = (ModelOptionsView)bindable;
            control.ItemSource = newList;
        }
    }

    private static void OnNotificationActionsChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var control = (ModelOptionsView)bindable;
        var list = newValue as ObservableCollection<DisplayedNotificationActions>;
        if(list != null)
        {
            control.NotificationActions = list;
        }
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="query"></param>
    /// <returns></returns>
    public void OnPerformSearch(string? query)
    {
        if (IsBusy)
            return;

        if (query == null)
            return;
        if (query == string.Empty && ReloadSourceCommand?.CanExecute(null) == true)
            ReloadSourceCommand.Execute(null);
        if (query.Count() < 3)
            return;

        foreach (var group in ItemSource)
        {
            group.RemoveAll(m => !(m.Model != null && m.Model.Name != null && m.Model.Name.Contains(query, StringComparison.InvariantCultureIgnoreCase)));
        }
        ItemSource.RemoveAll(g => g.Count == 0);
    }
}