using OpenStockApi.Core.Models.Searches;

namespace OpenStockApp.Core.Maui.Contracts.Services
{
    public interface IResultWindowService
    {
        event EventHandler<Result>? DisplayResult;
        event EventHandler? EndReached;

        Task LoadResultsFromServerAsync(int pageSize, int pageNumber, CancellationToken cancellationToken = default);
        Task LoadFilteredResultsFromServerAsync(int pageSize, int pageNumber, CancellationToken cancellationToken = default);
    }
}