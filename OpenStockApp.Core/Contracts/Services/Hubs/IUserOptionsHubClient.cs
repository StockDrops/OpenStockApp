using OpenStockApi.Core.Models.Products;
using OpenStockApi.Core.Models.Regions;
using OpenStockApi.Core.Models.Users;
namespace OpenStockApp.Core.Contracts.Services.Hubs;

public interface IUserOptionsHubClient : IBaseHubClient
{
    Task<IEnumerable<Model>> GetModels(Product product, CancellationToken cancellationToken = default);
    Task<IEnumerable<Product>> GetProducts(CancellationToken cancellationToken = default);
    Task<IEnumerable<Country>> GetCountries(CancellationToken cancellationToken = default);
    Task<IEnumerable<Retailer>> GetRetailersByCountry(Country country, CancellationToken cancellationToken = default);
    Task<UserOptions?> GetUserOptionsAsync(CancellationToken cancellationToken = default);
    Task<ModelOptions?> SaveModelOptions(ModelOptions modelOptions, CancellationToken cancellationToken = default);
    Task<UserOptions?> SaveUserOptionsAsync(UserOptions userOptions, CancellationToken cancellationToken = default);
    Task<User?> GetUserAsync(CancellationToken cancellationToken = default);
    Task<User?> SaveUserAsync(User user, CancellationToken cancellationToken = default);
    Task<Device?> GetDeviceAsync(string deviceName, CancellationToken cancellationToken = default);
}
