namespace OpenStockApp.Discord.Services.Exceptions
{
    public class InvalidDiscordWebhookServerException : Exception
    {
        public InvalidDiscordWebhookServerException() : base() { }
        public InvalidDiscordWebhookServerException(string message) : base(message) { }
    }
}
