using Microsoft.Toolkit.Mvvm.Input;
using OpenStockApi.Core.Models.Searches;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace OpenStockApp.ViewModels
{
    public interface INotificationsPageViewModel : IBaseConnectionViewModel
    {
        ICommand ApplyFilterSettings { get; set; }
        bool EndReached { get; set; }
        bool HasToApplyFilterSettings { get; set; }
        bool IsRefreshing { get; set; }
        double ListHeight { get; set; }
        ICommand LoadMoreCommand { get; set; }
        ICommand NavigateToPage { get; set; }
        ObservableCollection<Result> Results { get; set; }
        Action<Result>? ScrollTo { get; set; }
        ICommand TestNotificationCommand { get; set; }

        Task OnLoadMore(CancellationToken cancellationToken = default);
        Task OnNavigatedTo(CancellationToken cancellationToken = default);
        void OnNotificationReceived(object? sender, Result result);
        Task OnSendTestNotification(CancellationToken token = default);
        void RegisterEvents();
    }
}