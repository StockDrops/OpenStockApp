namespace OpenStockApp.Core.Contracts.Services.Users
{
    public interface ITokenRegistrationService
    {
        Task RegisterTokenAsync(string token, CancellationToken cancellationToken = default);
    }
}