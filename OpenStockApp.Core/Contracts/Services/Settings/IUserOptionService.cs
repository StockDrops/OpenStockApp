using OpenStockApi.Core.Models.Users;

namespace OpenStockApp.Core.Contracts.Services.Settings
{
    public interface IUserOptionsService
    {
        UserOptions? UserOptions { get; }
        ModelOptions? GetModelOptionsFromCache(long modelId);
        void SetUserOptions(UserOptions? userOptions);

        Task SaveCurrentUserOptionsAsync(CancellationToken cancellationToken = default);
        /// <summary>
        /// Initializes the service to load the user options from the server.
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task InitializeAsync(CancellationToken cancellationToken = default);
    }
}