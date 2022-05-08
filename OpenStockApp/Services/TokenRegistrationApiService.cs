using OpenStockApi.Core.Models.Users;
using OpenStockApp.Core.Contracts.Services.Api;
using OpenStockApp.Core.Contracts.Services.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Net.Http.Json;
using OpenStockApp.Core.Contracts.Services.Users;

namespace OpenStockApp.Services
{
    public class TokenRegistrationApiService : ITokenRegistrationApiService
    {
        private readonly IApiService apiService;
        private readonly IDeviceService deviceService;
        public TokenRegistrationApiService(IApiService apiService, IDeviceService deviceService)
        {
            this.apiService = apiService;
            this.deviceService = deviceService;
        }
        public async Task<TokenRegistrationResponse?> RegisterTokenAsync(string token, string? arn = null, CancellationToken cancellationToken = default)
        {
            var deviceType = deviceService.GetDeviceType();
            var url = $"api/tokens/register/{token}&deviceType={deviceType}&deviceName={DeviceInfo.Name}";
            if (arn != null)
            {
                url += $"&arn={arn}";
            }
            var request = new HttpRequestMessage(HttpMethod.Put, url);
            var response = await apiService.SendRequestAsync(request, cancellationToken).ConfigureAwait(false);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<TokenRegistrationResponse>(cancellationToken: cancellationToken).ConfigureAwait(false);
            }
            return null;
        }
    }
}
