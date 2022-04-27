using Microsoft.Toolkit.Mvvm.Input;
using OpenStockApi.Core.Models.Products;
using OpenStockApi.Core.Models.Regions;
using OpenStockApi.Core.Models.Users;
using OpenStockApp.Core.Maui.Models;
using OpenStockApp.Models.Users;
using System.Collections.ObjectModel;

namespace OpenStockApp.ViewModels.AlertSettings
{
    public interface IAlertSettingsViewModel
    {
        ObservableCollection<Country> Countries { get; set; }
        AsyncRelayCommand LoadModels { get; set; }
        AsyncRelayCommand LoadProducts { get; set; }
        AsyncRelayCommand LoadRetailers { get; set; }
        ObservableCollection<GroupedObversableModelOptions> Models { get; set; }
        ObservableCollection<DisplayedNotificationActions> NotificationActions { get; set; }
        AsyncRelayCommand<string> PerformSearch { get; set; }
        ObservableCollection<Product> Products { get; set; }
        ObservableCollection<RetailerOptions> Retailers { get; set; }
        AsyncRelayCommand SaveModelOptions { get; set; }
        Country SelectedCountry { get; set; }
        Product SelectedProduct { get; set; }
        ObservableCollection<GroupedObversableModelOptions> UnsearchedModels { get; }

        void LoadActions();
        Task OnCountrySelected(CancellationToken cancellationToken);
        void OnDisplayRetailerOptions(object? sender, RetailerOptions retailerOptions);
        Task OnLoadProducts(CancellationToken token = default);
        Task OnPerformSearch(string? query, CancellationToken cancellationToken = default);
        Task OnProductSelected(CancellationToken cancellationToken = default);
        Task OnSaveModelOptions(CancellationToken cancellationToken = default);
        void RegisterEvents();
    }
}