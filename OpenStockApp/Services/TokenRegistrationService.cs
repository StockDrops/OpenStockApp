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
        public TokenRegistrationService(ITokenRegistrationApiService tokenRegistrationApiService)
        {
            this.tokenRegistrationApiService = tokenRegistrationApiService;
        }
        public async Task RegisterTokenAsync(string token, CancellationToken cancellationToken = default)
        {
            string? oldArn = Preferences.Get("arn", "");
            if (oldArn == "")
                oldArn = null;
            var arn = await tokenRegistrationApiService.RegisterTokenAsync(token, arn: oldArn, cancellationToken);
            if (arn?.RegisteredArn != null)
                Preferences.Set("arn", arn.RegisteredArn);
        }
    }
}
