using TIKSN.Data;

namespace TIKSN.Web.Rest;

public interface IRestBulkRepository<TEntity, TIdentity> where TEntity : IEntity<TIdentity>
    where TIdentity : IEquatable<TIdentity>
{
    public Task AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken);

    public Task<TEntity> GetAsync(TIdentity id, CancellationToken cancellationToken);

    public Task<TEntity?> GetOrDefaultAsync(TIdentity id, CancellationToken cancellationToken);

    public Task RemoveAsync(TEntity entity, CancellationToken cancellationToken);

    public Task UpdateRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken);
}
