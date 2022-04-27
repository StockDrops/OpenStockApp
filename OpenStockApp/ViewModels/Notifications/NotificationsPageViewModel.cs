using Microsoft.Toolkit.Mvvm.Input;
using OpenStockApp.SignalR.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenStockApi.Core.Models.Searches;
using Microsoft.Extensions.Logging;
using OpenStockApp.Core.Contracts.Services;
using OpenStockApp.Core.Contracts.Services.Hubs;
using OpenStockApp.Core.Maui.Contracts.Services;
using OpenStockApp.Core.Contracts.Services.Settings;

/* Unmerged change from project 'OpenStockApp (net6.0-windows10.0.19041)'
Before:
using OpenStockApp.Core.Contracts.Services.Users;
After:
using OpenStockApp.Core.Contracts.Services.Users;
using OpenStockApp;
using OpenStockApp.ViewModels;
using OpenStockApp.ViewModels.Notifications;
*/
using OpenStockApp.Core.Contracts.Services.Users;
using System.Windows.Input;

namespace OpenStockApp.ViewModels.Notifications
{
    public class NotificationsPageViewModel : BaseConnectionViewModel, INotificationsPageViewModel
    {
        public static readonly BindableProperty ListHeightProperty =
            BindableProperty.Create(nameof(ListHeight), typeof(double), typeof(NotificationsPageViewModel), 0.0);

        public double ListHeight
        {
            get => (double)GetValue(ListHeightProperty);
            set => SetValue(ListHeightProperty, value);
        }

        public static readonly BindableProperty EndReachedProperty = BindableProperty.Create(nameof(EndReached), typeof(bool), typeof(NotificationsPageViewModel), false);

        public bool EndReached
        {
            get => (bool)GetValue(EndReachedProperty);
            set => SetValue(EndReachedProperty, value);
        }
        public static readonly BindableProperty IsRefreshingProperty = BindableProperty.Create(nameof(IsRefreshing), typeof(bool), typeof(NotificationsPageViewModel), false);
        public bool IsRefreshing
        {
            get => (bool)GetValue(IsRefreshingProperty);
            set => SetValue(IsRefreshingProperty, value);
        }

        public static readonly BindableProperty HasToApplyFilterSettingsProperty = BindableProperty.Create(nameof(HasToApplyFilterSettings), typeof(bool), typeof(NotificationsPageViewModel), false);

        public bool HasToApplyFilterSettings
        {
            get => (bool)GetValue(HasToApplyFilterSettingsProperty);
            set => SetValue(HasToApplyFilterSettingsProperty, value);
        }

        public ICommand NavigateToPage { get; set; }
        public ICommand LoadMoreCommand { get; set; }
        public ICommand ApplyFilterSettings { get; set; }
        public ICommand TestNotificationCommand { get; set; }



        public Action<Result>? ScrollTo { get; set; }


        public ObservableCollection<Result> Results { get; set; } = new ObservableCollection<Result>();

        private readonly INotificationHubService notificationService;
        private readonly IResultWindowService resultWindowService;
        private readonly IFilterService filterService;
        private readonly INotificationsHubClient notificationsHubClient;
        private readonly ILogger<NotificationsPageViewModel> logger;
        private const int pageSize = 10;
        public NotificationsPageViewModel(INotificationsHubClient notificationsHubClient,
            IResultWindowService resultWindowService,
            IFilterService filterService,
            INotificationHubService notificationService,
            IIdentityService identityService,
            ILogger<NotificationsPageViewModel> logger) : base(notificationsHubClient, identityService)
        {
            this.notificationService = notificationService;
            this.resultWindowService = resultWindowService;
            this.filterService = filterService;
            this.notificationsHubClient = notificationsHubClient;
            this.logger = logger;

            NavigateToPage = new AsyncRelayCommand(OnNavigatedTo);
            LoadMoreCommand = new AsyncRelayCommand(OnLoadMore);
            ApplyFilterSettings = new AsyncRelayCommand(OnDisplayFilteredToggled);
            TestNotificationCommand = new AsyncRelayCommand(OnSendTestNotification);
#if DEBUG
            IsLoggedIn = true;
#endif
            RegisterEvents();
        }
        public void RegisterEvents()
        {
            notificationService.NotificationReceived += OnNotificationReceived;
            resultWindowService.EndReached += OnEndReached;
            resultWindowService.DisplayResult += OnDisplayResult;
        }

        public async Task OnSendTestNotification(CancellationToken token = default)
        {
            await notificationsHubClient.SendTestNotification(token);
            await (Application.Current?.MainPage?.DisplayAlert("Enable the test model if you want to receive a ping", "You can enable it in the alert settings if you want the notification to ping, under the product 'RTX 3080', just search for 'test' when loading the RTX 3080s. If you enable the test model you will not only see the notification on this page but will also see a toast notification and see if when you toggle 'Apply Filter Settings'", "OK") ?? Task.FromResult(false));
        }

        public void OnNotificationReceived(object? sender, Result result)
        {
            if (HasToApplyFilterSettings)
            {
                if (filterService.CanShowNotification(result))
                    Dispatcher.Dispatch(() => Results.Insert(0, result));
            }
            else
            {
                Dispatcher.Dispatch(() => Results.Insert(0, result));
            }
            ScrollTo?.Invoke(result);
        }

        private void OnEndReached(object? sender, EventArgs e) => EndReached = true;

        private void OnDisplayResult(object? sender, Result result) => Results.Add(result);

        private async Task OnDisplayFilteredToggled(CancellationToken token = default)
        {
            IsRefreshing = true;
            EndReached = false;
            await LoadAndAddResultsAsync(1, HasToApplyFilterSettings, token);

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task OnNavigatedTo(CancellationToken cancellationToken = default)
        {
            try
            {
                if (IsLoggedIn)
                {
                    IsRefreshing = true;
                    EndReached = false;
                    await LoadAndAddResultsAsync(1, HasToApplyFilterSettings, cancellationToken);
                    IsRefreshing = false;
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "");
            }
        }
        protected override void OnLoggedIn(object? sender, EventArgs e)
        {
            base.OnLoggedIn(sender, e);
            _ = Task.Run(async () => await OnNavigatedTo());
        }
        protected override void OnLoggedOut(object? sender, EventArgs e)
        {
            base.OnLoggedOut(sender, e);
            Results.Clear();
        }
        private async Task LoadAndAddResultsAsync(int pageNumber, bool filtered, CancellationToken cancellationToken = default)
        {
            IsRefreshing = true;
            if (pageNumber == 1)
                Results.Clear();
            if (!filtered)
                await resultWindowService.LoadResultsFromServerAsync(pageSize, pageNumber, cancellationToken);
            else
                await resultWindowService.LoadFilteredResultsFromServerAsync(pageSize, pageNumber, cancellationToken);
            IsRefreshing = false;
        }

        private int previousPageNumber = 1;

        public async Task OnLoadMore(CancellationToken cancellationToken = default)
        {
            if (!EndReached)
            {
                IsRefreshing = true;
                previousPageNumber++;
                await LoadAndAddResultsAsync(previousPageNumber, HasToApplyFilterSettings, cancellationToken);
                IsRefreshing = false;
            }
        }
    }
}
