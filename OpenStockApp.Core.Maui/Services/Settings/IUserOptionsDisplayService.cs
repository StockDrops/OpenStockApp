using OpenStockApi.Core.Models.Products;
using OpenStockApp.Core.Maui.Models;

namespace OpenStockApp.Core.Maui.Services.Settings
{
    public interface IUserOptionsDisplayService
    {
        Task<IList<GroupedObversableModelOptions>> GetGroupedObversableModelOptionsAsync(Product product, CancellationToken cancellationToken);
        Task<IList<ObservableModelOptions>> GetObservableModelOptions(Product product, CancellationToken cancellationToken = default);
    }
}