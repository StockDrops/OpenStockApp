using OpenStockApi.Core.Models.Searches;
using OpenStockApi.Core.Models.Users;

namespace OpenStockApp.Core.Contracts.Services.Hubs;

public interface INotificationsHubClient : IBaseHubClient
{
    IAsyncEnumerable<Result> GetLatestResults(int pageSize, int pageNumber, CancellationToken cancellationToken = default);
    IAsyncEnumerable<Result> GetFilteredResults(UserOptions? userOptions, int pageSize, int pageNumber, CancellationToken cancellationToken = default);
    Task SendTestNotification(CancellationToken cancellationToken = default);
}
