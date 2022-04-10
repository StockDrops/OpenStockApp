using Bucket4Csharp.Core.Interfaces;
using Bucket4Csharp.Core.Models;
using Discord;
using Discord.Webhook;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OpenStockApi.Core.Models.Products;
using OpenStockApi.Core.Models.Regions;
using OpenStockApi.Core.Models.Searches;
using OpenStockApi.Core.Models.Users;
using OpenStockApp.Core.Contracts.Services;
using OpenStockApp.Core.Contracts.Services.Settings;
using OpenStockApp.Core.Contracts.Services.Users;
using OpenStockApp.Core.Models.Settings;
using OpenStockApp.Discord.Contracts.Services;
using OpenStockApp.Discord.Models;
using OpenStockApp.Discord.Services.Exceptions;
using OpenStockApp.Ef.Models;
using System.Net.Http.Json;

namespace OpenStockApp.Discord.Services
{
    public class DiscordWebhookService : IDiscordWebhookService, INotificationService
    {
        private readonly IDbContextFactory<AppDbContext> contextFactory;
        private readonly ILogger<DiscordWebhookService> logger;
        private readonly ISubscriptionService subscriptionService;
        private readonly ISettingsService settingsService;
        private readonly ISchedulingBucket schedulingBucket;
        private readonly HttpClient client;
        public DiscordWebhookService(ISubscriptionService subscriptionService,
            ISettingsService settingsService,
            IDbContextFactory<AppDbContext> dbContextFactory,
            HttpClient client,
            ILogger<DiscordWebhookService> logger)
        {
            
            this.settingsService = settingsService;
            this.subscriptionService = subscriptionService;
            contextFactory = dbContextFactory;
            this.client = client;
            this.logger = logger;
            schedulingBucket = (ISchedulingBucket)IBucket.CreateBuilder().AddLimit(Bandwidth.Simple(3, TimeSpan.FromSeconds(1))).Build();
        }
        ///<inheritdoc/>
        public async Task<ulong> TestDiscordWebhook(string discordWebhookUrl)
        {
            if(string.IsNullOrEmpty(discordWebhookUrl))
                throw new ArgumentNullException(nameof(discordWebhookUrl));
            var testResult = new Result
            {
                AtcUrl = "https://twitter.com/stock_drops",
                ImageUrl = "https://stockdrops.net/wp-content/uploads/2021/05/app.jpg",
                Price = new Price { Currency = new Currency { Code = "USD", CurrencySymbolLocation = CurrencySymbolLocation.Left, Name = "Dollar", Symbol = "$" }, Value = 1500M },
                ProductUrl = "https://discord.gg/stockdrops",
                Sku = new Sku { IsEnabled = true, ModelId = 1, Name = "B01MYWVP6O", Value = "B01MYWVP6O", RetailerId = 1, Retailer = new Retailer { Name = "Test Retailer", IsEnabled = true}, Model = new Model { Name = "Test Product", Id = 1, BrandId = 1, IsEnabled = true, ProductId = 1 } },
                StockMessage = "This is your test. You should have seen StockDrops' Discord open or our Twitter."
            };
            return await ExecuteDiscordWebhook(testResult, discordWebhookUrl);
        }
        ///<inheritdoc/>
        public async Task<ulong> ExecuteDiscordWebhook(Result result, string discordWebhookUrl)
        {
            await schedulingBucket.TryConsumeAsync(1, 1000, default);
            var webhookUri = new Uri(discordWebhookUrl);
            //TODO: enable this once out of beta.
            //if (await subscriptionService.GetSubscriptionLevelAsync() != SubscriptionLevels.Gold)
            //{
            //    throw new WrongSubscriptionException("This method requires a Gold subscription", SubscriptionLevels.Gold);
            //}
            var isValidDiscordServer = await ValidateWebhookServerAgainstBlacklist(webhookUri);
            if (!isValidDiscordServer)
            {
                // At this point we can silently fail.
                // The user should not have been able to add an invalid webhook via the settings page in the first place,
                // meaning they manually edited the Json settings file to get around the initial validation.
                throw new InvalidDiscordWebhookServerException("The server is in the blacklist.");
            }
            
            return await executeDiscordWebhook(result, CreateWebhookClient(discordWebhookUrl));
        }
        private DiscordWebhookClient CreateWebhookClient(string discordWebhookUrl)
        {
            return new DiscordWebhookClient(discordWebhookUrl);
        }
        private async Task<ulong> executeDiscordWebhook(Result result, DiscordWebhookClient webhookClient)
        {
            var message = "In stock: " + ResultToMessage(result, false);

            var embedBuilder = new EmbedBuilder();
            if (!string.IsNullOrEmpty(result.Sku?.Model?.Name))
                embedBuilder.WithTitle(result.Sku.Model.Name);
            else if (!string.IsNullOrEmpty(result.Sku?.Name))
                embedBuilder.WithTitle(result.Sku.Name);

            if (!string.IsNullOrEmpty(result.ProductUrl))
            {
                embedBuilder.AddField("Product Url", $"[Product Url]({result.ProductUrl})", true);
            }
            if (!string.IsNullOrEmpty(result.AtcUrl))
            {
                embedBuilder.AddField("Add to Cart Url", $"[ATC Url]({result.AtcUrl})", true);
            }
            embedBuilder.AddField("\u200b","[Provided by StockDrops](https://discord.gg/stockdrops)",false);
            embedBuilder.WithColor(Color.Green);
            embedBuilder.WithThumbnailUrl(result.ThumbnailUrl);
            embedBuilder.WithFooter("\u200b", "https://files.stockdrops.net/html/logo.png");

            return await webhookClient.SendMessageAsync(message, embeds: new List<Embed> { embedBuilder.Build() }, username: "StockDrops", avatarUrl: "https://files.stockdrops.net/html/logo.png");
        }
        private string ResultToMessage(Result result, bool OpenInBrowser)
        {
            var suffix = "";
            if (OpenInBrowser)
            {
                suffix = " Opened In Browser!";
            }
            //this method will convert a resultDTO into a message in the notification
            using var context = contextFactory.CreateDbContext();
            if(result.Sku != null)
            {
                //get the retailer name if it doesn't exist in the sku
                string? retailerName = result.Sku.Retailer?.Name;
                if(retailerName == null)
                {
                    retailerName = context.Retailers.Where(r => r.Id == result.Sku.RetailerId).Select(r => r.Name).FirstOrDefault();
                }
                

                if (result.Sku?.Model != null && !string.IsNullOrEmpty(result.Sku?.Model.Name) && !string.IsNullOrEmpty(retailerName))
                {
                    return $"{result.Sku.Model.Name} for {result.Price} at {retailerName}.{suffix}";
                }
                else
                {
                    //we need the local model
                    var model = context.Models.Where(m => m.Id == result.Sku.ModelId).FirstOrDefault();
                    if (model != null)
                    {
                        return $"{model.Name} is {result.StockMessage} at {retailerName}.{suffix}";
                    }
                    else if(result.Sku != null)
                    {
                        return $"Found item {result.Sku.Name} - {result.Sku.Value} : {result.StockMessage} at {retailerName}.{suffix}";
                    }
                }
            }
            throw new ArgumentException("Result couldn't be transalated to a message");
        }
        private async Task<bool> ValidateWebhookServerAgainstBlacklist(Uri webhookUri)
        {
            // The same URL used to execute the webhook with a POST can also be used to retrieve information
            // about the webhook with a GET.
            
            var getResponse = await client.SendAsync(new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = webhookUri
            });

            var webhook = await getResponse.Content.ReadFromJsonAsync<DiscordWebhook>();
            if(webhook == null || string.IsNullOrEmpty(webhook.Guild_Id))
                return false;

            var isValid = !DiscordWebhookServerBlacklist.Contains(webhook.Guild_Id);
            if (!isValid)
            {
                logger?.LogWarning("Blacklisted Discord Webhook Attempt", new Dictionary<string, string>
                {
                     {"guild_id", webhook.Guild_Id }
                });
            }
            return isValid;
        }

        public async Task<ulong> ExecuteDiscordWebhook(Result result)
        {
            if(settingsService.LoadSettingString(DiscordSettingsKeys.DiscordWebhookUrl, out var webhookUrl) && !string.IsNullOrEmpty(webhookUrl))
            {
                return await ExecuteDiscordWebhook(result, webhookUrl).ConfigureAwait(false);
            }
            throw new InvalidDiscordWebhookServerException("No saved webhook.");
        }

        public async Task SendNotificationAsync(IEnumerable<Result> results)
        {
            var tasks = new List<Task>();
            foreach(var result in results)
            {
                tasks.Add(ExecuteDiscordWebhook(result));
            }
            await Task.WhenAll(tasks).ConfigureAwait(false);
        }

        public async Task SendNotificationAsync(Result result)
        {
            try
            {
                await ExecuteDiscordWebhook(result).ConfigureAwait(false);
            }
            catch(Exception ex)
            {
                logger.LogError(ex, "");
            }
        }

        /// <summary>
        /// A list of Discord snowflake IDs for the guilds (aka servers) to blacklist 
        /// TODO: This list is also used in the DiscordWebhookRule when validating input on the settings page.
        /// Need to figure out how to use dependency injection in that class so this can come from the API
        /// instead of hardcoding a static list.
        /// </summary>
        public static List<string> DiscordWebhookServerBlacklist { get; set; } = new List<string>
        {
            "865600552222195723", // Tim's test server
            "755412431186165781", // NS
            "780884180547928084", // Fixitg
            "258327271952089090", // House of Carts
            "767566223729754122" // Falco
        };
    }
}
