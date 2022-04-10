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

namespace OpenStockApp.ViewModels.AlertSettings
{
    public class ObservableProduct : ObservableObject
    {
        private Product product;
        public ObservableProduct(Product product)
        {
            this.product = product;
        }

        public Product Product
        {
            get { return this.product; }
            set { SetProperty(ref this.product, value); } 
        }


    }

    public class AlertSettingsViewModel : BaseConnectionViewModel
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
        public bool IsLoggedIn => identityService.IsLoggedIn();
        public Product SelectedProduct { get; set; } = new Product();
        public Country SelectedCountry { get; set; } = new Country();
        #endregion

        #region Commands
        public AsyncRelayCommand LoadProducts { get; set; }
        public AsyncRelayCommand LoadModels { get; set; }
        public AsyncRelayCommand LoadRetailers { get; set; }

        public AsyncRelayCommand SaveModelOptions { get; set; }
        public AsyncRelayCommand<string> PerformSearch { get; set; }
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
        public AlertSettingsViewModel(IUserOptionsHubClient userOptionsHub,
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
            LoadProducts = new AsyncRelayCommand(OnLoadProducts);
            LoadRetailers = new AsyncRelayCommand(OnCountrySelected);
            LoadModels = new AsyncRelayCommand(OnProductSelected);
            SaveModelOptions = new AsyncRelayCommand(OnSaveModelOptions);
            PerformSearch = new AsyncRelayCommand<string>(OnPerformSearch);
            LogIn = new AsyncRelayCommand(OnLoggedIn);
            #endregion
            RegisterEvents();
        }
        public void RegisterEvents()
        {
            retailerOptionsDisplayService.DisplayRetailerOptions += OnDisplayRetailerOptions;
        }
        public void OnDisplayRetailerOptions(object? sender, RetailerOptions retailerOptions)
        {
            Retailers.Add(retailerOptions);
        }

        public async Task OnLoggedIn(CancellationToken cancellationToken = default)
        {
            await identityService.LoginAsync();
        }
        /// <summary>
        /// When a product is selected we load all the models.
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task OnProductSelected(CancellationToken cancellationToken = default)
        {
            await LoadModelsFromHub();
        }
        public async Task OnCountrySelected(CancellationToken cancellationToken)
        {
            await LoadCountryRetailers(SelectedCountry, cancellationToken);
        }
        public async Task OnLoadProducts(CancellationToken token = default)
        {
            await LoadAllCountries();
            await LoadAllProducts();
            LoadActions();
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
        }
        public async Task OnSaveModelOptions(CancellationToken cancellationToken = default)
        {
            IsBusy = true;
            try
            {
                await userOptionsService.SaveCurrentUserOptionsAsync(cancellationToken);
                MessagingCenter.Send<AlertSettingsViewModel, Exception?>(this, "saved", null);
            }
            catch (HubNotConnected ex)
            {
                MessagingCenter.Send<AlertSettingsViewModel, Exception?>(this, "saved", ex);
            }
            IsBusy = false;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="query"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task OnPerformSearch(string? query, CancellationToken cancellationToken = default)
        {
            if (IsBusy)
                return;

            if (query == null)
                return;
            if (query == string.Empty)
                await LoadModelsFromHub();
            if (query.Count() < 3)
                return;

            foreach (var group in Models)
            {
                group.RemoveAll(m => !(m.Model != null && m.Model.Name != null && m.Model.Name.Contains(query, StringComparison.InvariantCultureIgnoreCase)));
            }
            Models.RemoveAll(g => g.Count == 0);
        }

        private async Task LoadModelsFromHub(CancellationToken cancellationToken = default)
        {
            IsBusy = true;
            Models.Clear();
            _ = Task.Run(async () =>
            {
                await Task.Delay(1000);
                var groupedOptions = await userOptionsDisplayService.GetGroupedObversableModelOptionsAsync(SelectedProduct, cancellationToken);

                foreach (var options in groupedOptions)
                {
                    
                    Dispatcher.Dispatch( () => Models.Add(options));
                    await Task.Delay(200);
                }
                Dispatcher.Dispatch(() => IsBusy = false);
            });
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
        private async Task LoadAllProducts(CancellationToken token = default)
        {
            IsBusy = true;

            var products = await Task.Run(async () => await userOptionsHub.GetProducts(token));
            Products.Clear();
            foreach (var product in products)
            {
                Products.Add(product);
            }
            IsBusy = false;
        }
    }
}
