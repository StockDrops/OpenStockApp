using Microsoft.Maui.Dispatching;
using Microsoft.Toolkit.Mvvm.Input;
using OpenStockApi.Core.Models.Products;
using OpenStockApi.Core.Models.Regions;
using OpenStockApi.Core.Models.Users;
using OpenStockApp.Core.Maui.Models;
using OpenStockApp.Extensions;
using OpenStockApp.Models.Users;
using OpenStockApp.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace OpenStockApp.ViewModels.AlertSettings
{
    public class TestAlertSettingsViewModel : IAlertSettingsViewModel
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
        public Product SelectedProduct { get; set; } = new Product();
        public Country SelectedCountry { get; set; } = new Country();
        #endregion

        #region Commands
        public AsyncRelayCommand LoadProducts { get; set; }
        public AsyncRelayCommand ProductSelected { get; set; }
        public AsyncRelayCommand LoadRetailers { get; set; }

        public AsyncRelayCommand SaveModelOptions { get; set; }
        public AsyncRelayCommand<string> PerformSearch { get; set; }

        public ICommand ConnectCommand { get; }

        public bool IsConnected { get; }

        public bool IsLoggedIn { get; set; }

        public ICommand LogIn { get; }

        public bool IsBusy { get; set; }
        public string? Title { get; set; }
        #endregion
        //public AsyncRelayCommand SaveUserOptions { get; set; }

        //private readonly UserOptionsService userOptionsService;
        public TestAlertSettingsViewModel()
        {
            #region Command Assigments
            LoadProducts = new AsyncRelayCommand(OnLoadProducts);
            LoadRetailers = new AsyncRelayCommand(OnCountrySelected);
            ProductSelected = new AsyncRelayCommand(OnProductSelected);
            SaveModelOptions = new AsyncRelayCommand(OnSaveModelOptions);
            PerformSearch = new AsyncRelayCommand<string>(OnPerformSearch);
            #endregion
            IsLoggedIn = false;
            RegisterEvents();
        }
        public void RegisterEvents()
        {
            
        }

        public void OnDisplayRetailerOptions(object? sender, RetailerOptions retailerOptions)
        {
            Retailers.Add(retailerOptions);
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
            LoadActions();
           
            await LoadAllProducts();

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
                //await userOptionsService.SaveCurrentUserOptionsAsync(cancellationToken);
                IsBusy = false;
                MessagingCenter.Send<IAlertSettingsViewModel, Exception?>(this, "saved", null);
            }
            catch (HubNotConnected ex)
            {
                IsBusy = false;
                MessagingCenter.Send<IAlertSettingsViewModel, Exception?>(this, "saved", ex);
            }

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

            for(int i = 0; i < 4; i++)
            {
                
                var l = new List<ShowModel>();
                for(int j = 0; j < 8; j++)
                {
                    var testModel = new Model
                    {
                        Id = i,
                        IsEnabled = true,
                        IsFeatured = false,
                        Name = $"Test model {i}",
                        BrandId = 1,
                        Brand = new Brand { Name = "Test brand", IsEnabled = true },
                        Product = new Product { IsEnabled = true, Name = "Test Product" },

                    };
                    var modelOptions = new ModelOptions
                    {
                        ModelId = i,
                        Model = testModel,
                        IsEnabled = i % 2 == 0 ? true : false
                    };
                    await Task.Delay(50); //https://github.com/xamarin/Xamarin.Forms/issues/13268#issuecomment-857895339
                    l.Add(new ShowModel(testModel, modelOptions));
                }
                Dispatcher.GetForCurrentThread()?.Dispatch(() => Models.Add(new GroupedObversableModelOptions($"Test {i}", l)));
            }
            //_ = Task.Run(async () =>
            //{
            //    //await Task.Delay(1000);
            //    var groupedOptions = await userOptionsDisplayService.GetGroupedObversableModelOptionsAsync(SelectedProduct, cancellationToken);

            //    foreach (var options in groupedOptions)
            //    {

            //        Dispatcher.Dispatch(() => Models.Add(options));
            //        //await Task.Delay(200);
            //    }
            //    Dispatcher.Dispatch(() => IsBusy = false);
            //});
        }
        private async Task LoadCountryRetailers(Country country, CancellationToken token = default)
        {
            IsBusy = true;
            Retailers.Clear();
            //await retailerOptionsDisplayService.LoadAndDisplayRetailers(SelectedCountry, token);
            IsBusy = false;
        }
        private Task LoadAllProducts(CancellationToken token = default)
        {
            IsBusy = true;
            Products.Clear();
            for(int i = 0; i < 10; i++)
            {
                var product = new Product
                {
                    Name = $"Test product {i}",
                    Description = $"Description {i}",
                    IsEnabled = true,
                    Id = i,
                    IsFeatured = false

                };
                Products.Add(product);
            }
            IsBusy = false;
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            
        }

        public void OnClosed(object? sender, Exception? exception)
        {

        }

        public Task OnConnect(CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }

        public void OnConnected(object? sender, EventArgs eventArgs)
        {
            
        }

        public void OnReconnected(object? sender, EventArgs eventArgs)
        {
            
        }
    }
}
