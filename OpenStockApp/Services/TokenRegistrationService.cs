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
        private readonly ILogger<TokenRegistrationService> logger;
        public TokenRegistrationService(ITokenRegistrationApiService tokenRegistrationApiService, ILogger<TokenRegistrationService> logger)
        {
            this.tokenRegistrationApiService = tokenRegistrationApiService;
            this.logger = logger;
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
        }
    }
}
