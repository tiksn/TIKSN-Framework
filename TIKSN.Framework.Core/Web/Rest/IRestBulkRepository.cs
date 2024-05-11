using TIKSN.Data;

namespace TIKSN.Web.Rest;

public interface IRestBulkRepository<TEntity, TIdentity> where TEntity : IEntity<TIdentity>
    where TIdentity : IEquatable<TIdentity>
{
    Task AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken);

    Task<TEntity> GetAsync(TIdentity id, CancellationToken cancellationToken);

    Task<TEntity?> GetOrDefaultAsync(TIdentity id, CancellationToken cancellationToken);

    Task RemoveAsync(TEntity entity, CancellationToken cancellationToken);

    Task UpdateRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken);
}
