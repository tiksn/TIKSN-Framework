using TIKSN.Data;

namespace TIKSN.Web.Rest;

public interface IRestRepository<TEntity, TIdentity> where TEntity : IEntity<TIdentity>
    where TIdentity : IEquatable<TIdentity>
{
    public Task AddAsync(TEntity entity, CancellationToken cancellationToken);

    public Task<TEntity> GetAsync(TIdentity id, CancellationToken cancellationToken);

    public Task<TEntity?> GetOrDefaultAsync(TIdentity id, CancellationToken cancellationToken);

    public Task RemoveAsync(TEntity entity, CancellationToken cancellationToken);

    public Task UpdateAsync(TEntity entity, CancellationToken cancellationToken);
}
