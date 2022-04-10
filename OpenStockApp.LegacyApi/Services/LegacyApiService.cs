using Microsoft.Extensions.Logging;
using OpenStockApp.Core.Contracts.Services.Users;
using OpenStockApp.Core.Services.Api;
using OpenStockApp.LegacyApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http.Json;

namespace OpenStockApp.LegacyApi.Services
{
    public class LegacyApiService : ApiService
    {
        private readonly ILogger<LegacyApiService> logger;
        private readonly IIdentityService identityService;

        public LegacyApiService(IIdentityService identityService,
            HttpClient httpClient, 
            ILogger<LegacyApiService> logger) : base(identityService, httpClient)
        {
            this.logger = logger;
            this.identityService = identityService;
        }

        public async Task<SubscriptionDTO?> GetSubscriptionLevel(CancellationToken cancellationToken = default)
        {
            var user = await identityService.GetUserDefaultScopesAsync(cancellationToken);
            if(user != null)
            {
                SubscriptionDTO Result = new SubscriptionDTO();
                
                try
                {
                    var request = new HttpRequestMessage(HttpMethod.Get, $"api/users/subscription/{user.Id}");
                    var response = await SendRequestAsync(request);
                    if (response.IsSuccessStatusCode)
                    {
                        return await response.Content.ReadFromJsonAsync<SubscriptionDTO>().ConfigureAwait(false);
                    }
                }
                catch (Exception ex)
                {
                    logger?.LogError(ex, "");
                }
            }
            return null;
        }
    }
}
