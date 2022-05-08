using OpenStockApi.Core.Models.Users;

namespace OpenStockApp.Core.Contracts.Services.Users
{
    public interface ITokenRegistrationApiService
    {
        Task<TokenRegistrationResponse?> RegisterTokenAsync(string token, string? arn = null, CancellationToken cancellationToken = default);
    }
}