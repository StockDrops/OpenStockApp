using Microsoft.Toolkit.Mvvm.Input;
using OpenStockApi.Core.Models.Products;
using OpenStockApi.Core.Models.Regions;
using OpenStockApi.Core.Models.Users;
using OpenStockApp.Core.Maui.Models;
using OpenStockApp.Models.Users;
using System.Collections.ObjectModel;

namespace OpenStockApp.ViewModels.AlertSettings
{
    public interface IAlertSettingsViewModel : IBaseConnectionViewModel
    {
        public ObservableCollection<Country> Countries { get; set; }
        public AsyncRelayCommand ProductSelected { get; set; }
        public AsyncRelayCommand LoadProducts { get; set; }
        public ObservableCollection<GroupedObversableModelOptions> Models { get; set; }
        public ObservableCollection<DisplayedNotificationActions> NotificationActions { get; set; }
        public AsyncRelayCommand<string> PerformSearch { get; set; }
        public ObservableCollection<Product> Products { get; set; }
        public ObservableCollection<RetailerOptions> Retailers { get; set; }
        public AsyncRelayCommand SaveModelOptions { get; set; }
        public Product SelectedProduct { get; set; }
        public ObservableCollection<GroupedObversableModelOptions> UnsearchedModels { get; }

        public Task OnCountrySelected(CancellationToken cancellationToken);
        public void OnDisplayRetailerOptions(object? sender, RetailerOptions retailerOptions);
        public Task OnLoadProducts(CancellationToken token = default);
        public Task OnPerformSearch(string? query, CancellationToken cancellationToken = default);
        public Task OnProductSelected(CancellationToken cancellationToken = default);
        public Task OnSaveModelOptions(CancellationToken cancellationToken = default);
    }
}