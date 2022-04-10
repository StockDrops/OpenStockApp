using OpenStockApp.Core.Contracts.Services.Api;
using OpenStockApp.Core.Contracts.Services.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenStockApp.Core.Services.Api
{
    public class NotLoggedInException : Exception
    {

    }
    public class ApiService : IApiService
    {
        private readonly IIdentityService identityService;
        private readonly HttpClient httpClient;
        public ApiService(IIdentityService identityService,
            HttpClient httpClient)
        {
            this.identityService = identityService;
            this.httpClient = httpClient;
        }
        /// <summary>
        /// Sends a request to our api using the obtained token.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        /// <exception cref="NotLoggedInException"></exception>
        public async Task<HttpResponseMessage> SendRequestAsync(HttpRequestMessage request, CancellationToken cancellationToken = default)
        {
            if (!identityService.IsLoggedIn())
                throw new NotLoggedInException();

            SetAuthHeader(await identityService.GetAccessTokenAsync(cancellationToken));

            return await httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);
        }
        private void SetAuthHeader(string? token)
        {
            if(token == null)
                throw new ArgumentNullException(nameof(token));

            httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        }
    }
}
