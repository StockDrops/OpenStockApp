using Microsoft.Toolkit.Diagnostics;
using Microsoft.Toolkit.Mvvm.Input;
using OpenStockApi.Core.Models.Products;
using OpenStockApi.Core.Models.Users;
using OpenStockApp.Core.Contracts.Services.Hubs;
using OpenStockApp.Core.Contracts.Services.Settings;
using OpenStockApp.Core.Contracts.Services.Users;
using OpenStockApp.Core.Maui.Models;
using OpenStockApp.Models.Users;
using OpenStockApp.Pages.Alerts;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenStockApp.ViewModels.AlertSettings
{
    public class ActiveNotificationsViewModel : BaseConnectionViewModel
    {
        public ObservableCollection<GroupedObversableModelOptions> GroupedModelOptions { get; set; } = new ObservableCollection<GroupedObversableModelOptions>();
        public ObservableCollection<Product> Products { get; set; } = new ObservableCollection<Product>();
        public ObservableCollection<DisplayedNotificationActions> NotificationActions { get; set; } = new ObservableCollection<DisplayedNotificationActions>();
        public Product SelectedProduct { get; set; } = new Product();
        public Command NavigatedToCommand { get; set; }
        public AsyncRelayCommand ItemSelected { get; set; }
        public AsyncRelayCommand ReloadModels { get; set; }
        public AsyncRelayCommand SaveModelOptions { get; set; }

        private readonly IUserOptionsService userOptionsService;
        private readonly IIdentityService identityService;
        public ActiveNotificationsViewModel(IBaseHubClient baseHubClient, 
            IUserOptionsService userOptionsService,
            IIdentityService identityService) : base(baseHubClient, identityService)
        {
            this.userOptionsService = userOptionsService;
            NavigatedToCommand = new Command(() => OnNavigatedTo());
            ItemSelected = new AsyncRelayCommand(OnProductSelected);
            ReloadModels = new AsyncRelayCommand(OnReloadSource);
            SaveModelOptions = new AsyncRelayCommand(OnSaveModelOptions);
            this.identityService = identityService;
#if ANDROID
            MessagingCenter.Subscribe<ActiveNotificationsPage>(this, "NavigatedTo", (sender) => Dispatcher.Dispatch(() => OnNavigatedTo()));
#endif
        }
        public void OnNavigatedTo()
        {
            GroupedModelOptions.Clear();
            LoadProducts();
            LoadActions();
        }
        private void LoadActions(CancellationToken cancellationToken = default)
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
            await userOptionsService.SaveCurrentUserOptionsAsync(cancellationToken);
            IsBusy = false;
        }
        private void LoadProducts()
        {
            if (identityService.IsLoggedIn())
            {
                var l = userOptionsService.UserOptions?.ModelOptions.Where(m => m.IsEnabled).Select(m => m.Model.Product).DistinctBy(p => p.Id).ToList();
                Products.Clear();
                Guard.IsNotNull(l, "Product List");
                foreach (var product in l)
                {
                    if (product != null)
                        Products.Add(product);
                }
            }
        }
        private async Task OnReloadSource(CancellationToken cancellationToken = default)
        {
            if (SelectedProduct is not null)
            {
                await LoadActiveNotifications(SelectedProduct, cancellationToken);
            }
        }
        /// <summary>
        /// When a product is selected we load the models.
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task OnProductSelected(CancellationToken cancellationToken = default)
        {
            if(SelectedProduct is not null)
            {
                await LoadActiveNotifications(SelectedProduct, cancellationToken);
            }
        }

        public async Task LoadActiveNotifications(Product product, CancellationToken cancellationToken = default)
        {
            var modelOptionsForProduct = userOptionsService.UserOptions?.ModelOptions.Where(m => m.IsEnabled && m.Model.ProductId == product.Id).ToList();
            if(modelOptionsForProduct is not null)
            {
                GroupedModelOptions.Clear();
                var observableModels = new List<ObservableModelOptions>();
                foreach (var modelOptions in modelOptionsForProduct)
                {
                    if(modelOptions != null && modelOptions.Model != null)
                    {
                        observableModels.Add(new ObservableModelOptions(modelOptions.Model, modelOptions));
                    }
                }
                var l = observableModels.GroupBy(m => m.Model.BrandId)
                                                    .OrderBy(g => g.First().Model?.Brand?.Name)
                                                    .Select(x => new GroupedObversableModelOptions(x.First().Model?.Brand?.Name ?? "Null", x.ToList()))
                                                    .ToList();
                foreach(var item in l)
                {
                    GroupedModelOptions.Add(item);
                }
            }
        }
    }
}
