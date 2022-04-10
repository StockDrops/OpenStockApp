using OpenStockApi.Core.Models.Users;

namespace OpenStockApp.Core.Contracts.Services.Users
{
    public interface ISubscriptionService
    {
        Task<SubscriptionLevels> GetSubscriptionLevelAsync();
    }
}
