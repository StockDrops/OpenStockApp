using OpenStockApi.Core.Contracts.Models;
using OpenStockApp.Core.Contracts.Services.Api;
using System.Net.Http.Json;

namespace OpenStockApp.Core.Services.Api
{
    /// <summary>
    /// Generic entity api service to be used to get/save/delete/update entities with long id's.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class SelfEntityApiService<T> : EntityApiService<T>, IUserEntityApiService<T> 
        where T : IUserLinkedEntity
    {
        private readonly IApiService apiService;
        private readonly string apiPath;
        public SelfEntityApiService(IApiService apiService, string apiPath) : base(apiService, apiPath)
        {
            this.apiService = apiService;
            this.apiPath = apiPath;
        }
        /// <summary>
        /// Gets an entity by Id.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        /// <exception cref="HttpRequestException"></exception>
        public async Task<T?> GetSelfEntityAsync(CancellationToken token = default)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, $"{apiPath}/self");
            var response = await apiService.SendRequestAsync(request);

            var str = await response.Content.ReadAsStringAsync();

            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<T>(cancellationToken: token).ConfigureAwait(false);
        }
        /// <summary>
        /// Gets all associated enities.
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public async Task<IList<T>> GetAllSelfEntitiesAsync(CancellationToken token = default)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, $"{apiPath}");
            var response = await apiService.SendRequestAsync(request);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<List<T>>(cancellationToken: token) ?? new List<T>();
        }
        /// <summary>
        /// Save an entity through the api. Make sure you have the right scopes or this will throw an HttpRequestException with 40x.
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task SaveSelfAsync(T entity, CancellationToken cancellationToken = default)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, $"{apiPath}/self");
            request.Content = JsonContent.Create<T>(entity);
            var response = await apiService.SendRequestAsync(request, cancellationToken);
            response.EnsureSuccessStatusCode();
        }
        /// <summary>
        /// Delete an entity with the given Id.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task DeleteSelfAsync(CancellationToken cancellationToken = default)
        {
            var request = new HttpRequestMessage(HttpMethod.Delete, $"{apiPath}/self");
            var response = await apiService.SendRequestAsync(request, cancellationToken);
            response.EnsureSuccessStatusCode();
        }
        /// <summary>
        /// Update the entity using the id in the entity given. It will replace the existing entity with the given entity.
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task UpdateSelfAsync(T entity, CancellationToken cancellationToken = default)
        {
            var request = new HttpRequestMessage(HttpMethod.Put, $"{apiPath}/{entity.Id}");
            var response = await apiService.SendRequestAsync(request, cancellationToken);
            response.EnsureSuccessStatusCode();
        }
    }
}
