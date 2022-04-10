using OpenStockApi.Core.Contracts.Models;

namespace OpenStockApp.Core.Contracts.Services.Api
{
    public interface IEntityApiService<T> where T : IEntity
    {
        Task DeleteAsync(long id, CancellationToken cancellationToken = default);
        Task<IList<T>> GetAllEntitiesAsync(CancellationToken token = default);
        Task<T?> GetEntityAsync(long id, CancellationToken token = default);
        Task SaveAsync(T entity, CancellationToken cancellationToken = default);
        Task UpdateAsync(T entity, CancellationToken cancellationToken = default);
    }
    public interface IUserEntityApiService<T> : IEntityApiService<T>
        where T : IUserLinkedEntity
    {
        Task DeleteSelfAsync(CancellationToken cancellationToken = default);
        Task<IList<T>> GetAllSelfEntitiesAsync(CancellationToken token = default);
        Task<T?> GetSelfEntityAsync(CancellationToken token = default);
        Task SaveSelfAsync(T entity, CancellationToken cancellationToken = default);
        Task UpdateSelfAsync(T entity, CancellationToken cancellationToken = default);
    }
}