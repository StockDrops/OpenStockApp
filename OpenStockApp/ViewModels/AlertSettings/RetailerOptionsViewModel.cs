using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using OpenStockApi.Core.Models.Products;
using OpenStockApi.Core.Models.Users;
using OpenStockApp.SignalR.Services;
using OpenStockApp.Extensions;
using System.Collections.ObjectModel;
using OpenStockApp.Core.Contracts.Services.Users;
using OpenStockApp.Core.Contracts.Services;
using OpenStockApi.Core.Models.Regions;
using OpenStockApp.Core.Contracts.Services.Hubs;
using OpenStockApp.Core.Maui.Models;
using OpenStockApp.Core.Maui.Services.Settings;
using OpenStockApp.Core.Contracts.Services.Settings;
using OpenStockApp.Models.Users;
using OpenStockApp.Services;
using OpenStockApp.Pages.Alerts;

namespace OpenStockApp.ViewModels.AlertSettings
{
    public class RetailerOptionsViewModel : BaseConnectionViewModel
    {
        #region Collections
        public ObservableCollection<Product> Products { get; set; } = new ObservableCollection<Product>();
        public ObservableCollection<Country> Countries { get; set; } = new ObservableCollection<Country>();
        public ObservableCollection<GroupedObversableModelOptions> Models { get; set; } = new ObservableCollection<GroupedObversableModelOptions>();
        public ObservableCollection<GroupedObversableModelOptions> UnsearchedModels { get; private set; } = new ObservableCollection<GroupedObversableModelOptions>();

        public ObservableCollection<RetailerOptions> Retailers { get; set; } = new ObservableCollection<RetailerOptions>();
        public ObservableCollection<DisplayedNotificationActions> NotificationActions { get; set; } = new ObservableCollection<DisplayedNotificationActions>();
        #endregion

        #region Properties
        public BindableProperty IsLoggedInProperty = BindableProperty.Create(nameof(IsLoggedIn), typeof(bool), typeof(AlertSettingsViewModel));
        public bool IsLoggedIn
        {
            get => (bool)GetValue(IsLoggedInProperty);
            set => SetValue(IsLoggedInProperty, value);
        }
        public Product SelectedProduct { get; set; } = new Product();
        public Country SelectedCountry { get; set; } = new Country();
        #endregion

        #region Commands
        public AsyncRelayCommand OnNavigatedToCommand { get; set; }
        public AsyncRelayCommand LoadRetailers { get; set; }

        public AsyncRelayCommand SaveModelOptions { get; set; }
        public AsyncRelayCommand LogIn { get; set; }
        #endregion
        
        #region Services
        private readonly IUserOptionsHubClient userOptionsHub;
        private readonly IIdentityService identityService;
        private readonly IUserOptionsDisplayService userOptionsDisplayService;
        private readonly IRetailerOptionsDisplayService retailerOptionsDisplayService;
        private readonly IUserOptionsService userOptionsService;
        #endregion
        //public AsyncRelayCommand SaveUserOptions { get; set; }

        //private readonly UserOptionsService userOptionsService;
        public RetailerOptionsViewModel(IUserOptionsHubClient userOptionsHub,
            IUserOptionsService userOptionsService,
            IIdentityService identityService,
            IRetailerOptionsDisplayService retailerOptionsDisplayService,
            IUserOptionsDisplayService userOptionsDisplayService)  : base(baseHubClient: userOptionsHub)
        {
            #region Service Assignements
            this.userOptionsHub = userOptionsHub;
            this.userOptionsDisplayService = userOptionsDisplayService;
            this.retailerOptionsDisplayService = retailerOptionsDisplayService;
            this.userOptionsService = userOptionsService;
            this.identityService = identityService;
            #endregion
            #region Command Assigments
            OnNavigatedToCommand = new AsyncRelayCommand(OnNavigatedTo);
            LoadRetailers = new AsyncRelayCommand(OnCountrySelected);
            SaveModelOptions = new AsyncRelayCommand(OnSaveModelOptions);
            LogIn = new AsyncRelayCommand(OnLoggedInRequest);

            

            #endregion
            IsLoggedIn = identityService.IsLoggedIn();
            RegisterEvents();
        }
        public void RegisterEvents()
        {
            retailerOptionsDisplayService.DisplayRetailerOptions += OnDisplayRetailerOptions;
            identityService.LoggedIn += OnLoggedIn;
            identityService.LoggedOut += OnLoggedOut;

            MessagingCenter.Subscribe<RetailerOptionsPage>(this, "NavigatedTo", async (sender) => await OnNavigatedTo());
        }


        public void OnDisplayRetailerOptions(object? sender, RetailerOptions retailerOptions)
        {
            Retailers.Add(retailerOptions);
        }
        public void OnLoggedIn(object? sender, EventArgs e)
        {
            IsLoggedIn = true;
        }
        public void OnLoggedOut(object? sender, EventArgs e)
        {
            IsLoggedIn = false;
        }

        public async Task OnLoggedInRequest(CancellationToken cancellationToken = default)
        {
            await identityService.LoginAsync();

        }
        public async Task OnCountrySelected(CancellationToken cancellationToken)
        {
            await LoadCountryRetailers(SelectedCountry, cancellationToken);
        }
        public async Task OnNavigatedTo(CancellationToken token = default)
        {
            LoadActions();
            await LoadAllCountries();
            //await LoadAllProducts();
            
        }
        public void LoadActions()
        {
            NotificationActions.Clear();
            NotificationActions.Add(new DisplayedNotificationActions
            {
                Action = NotificationAction.OpenProductUrl,
                Description = Resources.Strings.Resources.OpenProductUrl
            });
            NotificationActions.Add(new DisplayedNotificationActions
            {
                Action = NotificationAction.OpenAddToCartUrl,
                Description = Resources.Strings.Resources.OpenAddToCartUrl
            });
            NotificationActions.Add(new DisplayedNotificationActions
            {
                Action = NotificationAction.DoNothing,
                Description = Resources.Strings.Resources.DoNothingActionText
            });
        }
        public async Task OnSaveModelOptions(CancellationToken cancellationToken = default)
        {
            IsBusy = true;
            try
            {
                await userOptionsService.SaveCurrentUserOptionsAsync(cancellationToken);
                MessagingCenter.Send<RetailerOptionsViewModel, Exception?>(this, "saved", null);
            }
            catch (HubNotConnected ex)
            {
                MessagingCenter.Send<RetailerOptionsViewModel, Exception?>(this, "saved", ex);
            }
            IsBusy = false;
        }
        private async Task LoadAllCountries(CancellationToken token = default)
        {
            IsBusy = true;
            var countries = await userOptionsHub.GetCountries(token);
            Countries.Clear();
            foreach(var country in countries)
            {
                Countries.Add(country);
            }
            IsBusy = false;
        }
        private async Task LoadCountryRetailers(Country country, CancellationToken token = default)
        {
            IsBusy = true;
            Retailers.Clear();
            await retailerOptionsDisplayService.LoadAndDisplayRetailers(SelectedCountry, token);
            IsBusy = false;
        }
    }
}
