using OpenStockApi.Core.Models.Users;

namespace OpenStockApp.Discord.Services.Exceptions
{
    public class WrongSubscriptionException : Exception
    {
        public WrongSubscriptionException() : base() { }
        public WrongSubscriptionException(string message, SubscriptionLevels subscription) : base(message) { MinimumSubscriptionLevelNeeded = subscription; }
        public SubscriptionLevels MinimumSubscriptionLevelNeeded { get; private set; }
    }
}
