using Microsoft.Toolkit.Mvvm.Input;
using OpenStockApi.Core.Models.Products;
using OpenStockApi.Core.Models.Regions;
using OpenStockApi.Core.Models.Searches;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Condition = OpenStockApi.Core.Models.Products.Condition;

namespace OpenStockApp.ViewModels.Notifications
{
    internal class TestNotificationViewModel : INotificationsPageViewModel
    {
        public TestNotificationViewModel()
        {
            LoadMoreCommand = new Command(() => LoadResults());
            NavigateToPage = new Command(() => LoadResults());
        }

        private void LoadResults()
        {
            var testModel = new Model
            {
                IsEnabled = true,
                IsFeatured = false,
                Name = "Test model",
                BrandId = 1,
                Brand = new Brand { Name = "Test brand", IsEnabled = true},
                Product = new Product { IsEnabled = true, Name = "Test Product"},
                
            };
            var test = new Result
            {
                Id = 45181,
                ProductUrl = "https://stockdrops.net",
                AtcUrl = "https://discord.gg/stockdrops",
                Condition = new Condition { Description = "New", ProductCondition = ProductCondition.New },
                DateTimeFound = DateTime.UtcNow,
                ImageUrl = "https://files.stockdrops.net/html/logo.png",
                ThumbnailUrl = "https://developer.bestbuy.com/images/bestbuy-logo.png",
                Price = new Price { Currency = new Currency { Code = "USD", CurrencySymbolLocation = CurrencySymbolLocation.Left, Name = "Dollars", Symbol = "$", Id = 1 }, CurrencyId = 1, Value = 500 },
                Sku = new Sku
                {
                    Id = 1,
                    IsEnabled = true,
                    Model = testModel,
                    ModelId = testModel.Id,
                    Name = testModel.Name,
                    Retailer = null,
                    RetailerId = 1,
                    Value = "TEST MODEL"
                },
                StockMessage = "TEST MODEL IN STOCK",
                SkuId = 1
            };

            for (var i = 0; i < 10; i++)
            {
                Results.Add(test);
            }
        }

        public ICommand ApplyFilterSettings { get; set; }
        public bool EndReached { get; set; }
        public bool HasToApplyFilterSettings { get; set; }
        public bool IsRefreshing { get; set; }
        public double ListHeight { get; set; }
        public ICommand LoadMoreCommand { get; set; }
        public ICommand NavigateToPage { get; set; }
        public ObservableCollection<Result> Results { get; set; } = new ObservableCollection<Result>();
        public Action<Result>? ScrollTo { get; set; }
        public ICommand TestNotificationCommand { get; set; }

        public ICommand ConnectCommand { get; }

        public bool IsConnected { get; }

        public bool IsLoggedIn { get; set; } = true;

        public ICommand LogIn { get; }

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

        public Task OnLoadMore(CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }

        public Task OnNavigatedTo(CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }

        public void OnNotificationReceived(object? sender, Result result)
        {
            
        }

        public void OnReconnected(object? sender, EventArgs eventArgs)
        {
            
        }

        public Task OnSendTestNotification(CancellationToken token = default)
        {
            return Task.CompletedTask;
        }

        public void RegisterEvents()
        {
            
        }
    }
}
