using OpenStockApi.Core.Models.Searches;
using System.Net.Http;
using System.Threading.Tasks;

namespace OpenStockApp.Discord.Contracts.Services
{
    public interface IDiscordWebhookService
    {
        /// <summary>
        /// Executes the discord webhook using the saved uri.
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        Task<ulong> ExecuteDiscordWebhook(Result result);
        /// <summary>
        /// This method will send a notification to the given discord webhook url.
        /// Throws 
        /// </summary>
        /// <param name="resultDTO">Result to be parsed into an embed.</param>
        /// <param name="discordWebhookUrl">Webhook Url to send the notification to.</param>
        /// <exception cref="Discord.Services.Exceptions.WrongSubscriptionException">Throws this exception when the user has the wrong subscription for using the discord webhook.</exception>
        /// <exception cref="Discord.Services.Exceptions.InvalidDiscordWebhookServerException">Throws this exception when the server is in the blacklist.</exception>
        /// <returns></returns>
        Task<ulong> ExecuteDiscordWebhook(Result result, string discordWebhookUrl);
        /// <summary>
        /// This method will send a test notification to the given discord webhook url.
        /// Throws 
        /// </summary>
        /// <param name="discordWebhookUrl">Webhook Url to send the notification to.</param>
        /// <exception cref="Discord.Services.Exceptions.WrongSubscriptionException">Throws this exception when the user has the wrong subscription for using the discord webhook.</exception>
        /// <returns></returns>
        Task<ulong> TestDiscordWebhook(string discordWebhookUrl);
    }
}