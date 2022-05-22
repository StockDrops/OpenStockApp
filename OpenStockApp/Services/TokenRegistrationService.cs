using Microsoft.Extensions.Logging;
using OpenStockApp.Core.Contracts.Services.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenStockApp.Services
{
    public class TokenRegistrationService : ITokenRegistrationService
    {
        private readonly ITokenRegistrationApiService tokenRegistrationApiService;
        private readonly IIdentityService identityService;
        private readonly ILogger<TokenRegistrationService> logger;
        public TokenRegistrationService(IIdentityService identityService, ITokenRegistrationApiService tokenRegistrationApiService, ILogger<TokenRegistrationService> logger)
        {
            this.identityService = identityService;
            this.tokenRegistrationApiService = tokenRegistrationApiService;
            this.logger = logger;

            identityService.LoggedIn += OnLoggedIn;
        }
        public async void OnLoggedIn(object? sender, EventArgs e)
        {
            var token = Preferences.Get("token", "");
            if (!string.IsNullOrEmpty(token))
            {
                await RegisterTokenAsync(token);
            }
        }
        public async Task RegisterTokenAsync(string token, CancellationToken cancellationToken = default)
        {
            try
            {
                string? oldArn = Preferences.Get("arn", "");
                if (oldArn == "")
                    oldArn = null;
                var arn = await tokenRegistrationApiService.RegisterTokenAsync(token, arn: oldArn, cancellationToken);
                if (arn?.RegisteredArn != null)
                    Preferences.Set("arn", arn.RegisteredArn);
            }
            catch(Exception e) 
            {
                logger.LogError(e, "");
                System.Diagnostics.Debug.WriteLine(e.Message);
            }
            finally
            {
                Preferences.Set("token", token);
            }
        }
    }
}
