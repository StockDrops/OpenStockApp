using Microsoft.Extensions.Logging;
using Microsoft.Toolkit.Mvvm.Input;
using MvvmHelpers;
using OpenStockApp.Core.Contracts.Services.Settings;
using OpenStockApp.Core.Models.Settings;
using OpenStockApp.Discord.Contracts.Services;
using OpenStockApp.Discord.Services.Exceptions;
using OpenStockApp.Models;
using OpenStockApp.Resources.Strings;

namespace OpenStockApp.ViewModels.Settings
{
    
    public class DiscordSettingsViewModel : ObservableObject
    {
        public Command UpdateWebhookCommand { get; set; }
        public AsyncRelayCommand TestWebhookCommand { get; private set; }
        public Command ClearConfirmationMessageCommand { get; private set; }

        private string webhookUrl = "";
        public string WebhookUrl
        {
            get => webhookUrl;
            set => SetProperty(ref webhookUrl, value);
        }
        public ConfirmationMessage ConfirmationMessage { get; set; } = new ConfirmationMessage();

        private string? errorMessage;
        public string? ErrorMessage
        {
            get => errorMessage;
            set => SetProperty(ref errorMessage, value);
        }

        private readonly ISettingsService settingsService;
        private readonly IDiscordWebhookService discordWebhookService;
        private readonly ILogger<DiscordSettingsViewModel> logger;
        public DiscordSettingsViewModel(ISettingsService settingsService,
            IDiscordWebhookService discordWebhookService,
            ILogger<DiscordSettingsViewModel> logger)
        {
            UpdateWebhookCommand = new Command(UpdateWebhook);
            TestWebhookCommand = new AsyncRelayCommand(TestWebhookAsync);
            ClearConfirmationMessageCommand = new Command(ClearConfirmationMessage);


            this.settingsService = settingsService;
            this.discordWebhookService = discordWebhookService;
            this.logger = logger; 
            
            LoadWebhookUrl();
        }
        private void ClearConfirmationMessage()
        {
            ConfirmationMessage.Message = "";
            ConfirmationMessage.IsVisible = false;
        }
        public async Task TestWebhookAsync()
        {
            try
            {
                var response = await discordWebhookService.TestDiscordWebhook(WebhookUrl);

                ConfirmationMessage.Message = DiscordResources.DiscordWebhookMessageSentSuccesfully;
                ConfirmationMessage.IsVisible = true;

            }
            catch (ArgumentNullException)
            {
                ErrorMessage = DiscordResources.DiscordWebhookEmptyErrorMessage;
            }
            catch (WrongSubscriptionException ex)
            {
                ErrorMessage = string.Format(DiscordResources.DiscordWebhookSubscriptionErrorMessage, ex.MinimumSubscriptionLevelNeeded.ToString());
                return;
            }
            catch (InvalidDiscordWebhookServerException)
            {
                ConfirmationMessage.Message = "Failed. The Webhook Uri is not valid.";
                ConfirmationMessage.IsVisible = true;
            }
            catch (Exception ex)
            {
                logger?.LogError(ex, "");
                ErrorMessage = DiscordResources.DiscordWebhookDefaultErrorMessage;
            }

        }
        public void UpdateWebhook()
        {
            if (settingsService.SaveSettingString(DiscordSettingsKeys.DiscordWebhookUrl, WebhookUrl))
            {
                ConfirmationMessage.Message = DiscordResources.WebhookSavedSuccesfully;
                ConfirmationMessage.IsVisible = true;
            }
            else
            {
                ConfirmationMessage.Message = DiscordResources.WebhookSavedError;
                ConfirmationMessage.IsVisible = true;
            }
        }
        private void LoadWebhookUrl()
        {
            if (settingsService.LoadSettingString(DiscordSettingsKeys.DiscordWebhookUrl, out var webhook))
            {
                WebhookUrl = webhook ?? string.Empty;
            }
        }
    }
}
