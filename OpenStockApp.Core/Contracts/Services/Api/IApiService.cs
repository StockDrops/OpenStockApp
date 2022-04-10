using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenStockApp.Core.Contracts.Services.Api
{
    public interface IApiService
    {
        /// <summary>
        /// Sends a request to our API.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        /// <exception cref="OpenStockApp.Core.Services.Api.NotLoggedInException">If the user is not logged in</exception>
        public Task<HttpResponseMessage> SendRequestAsync(HttpRequestMessage request, CancellationToken cancellationToken = default);
    }
}
