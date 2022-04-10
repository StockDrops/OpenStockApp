using OpenStockApi.Core.Models.Regions;
using OpenStockApi.Core.Models.Users;

namespace OpenStockApp.Core.Maui.Services.Settings
{
    public interface IRetailerOptionsDisplayService
    {
        event EventHandler<RetailerOptions>? DisplayRetailerOptions;

        Task LoadAndDisplayRetailers(Country country, CancellationToken token = default);
    }
}