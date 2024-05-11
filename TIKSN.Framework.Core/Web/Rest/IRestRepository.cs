using TIKSN.Data;

namespace TIKSN.Web.Rest;

public interface IRestRepository<TEntity, TIdentity> where TEntity : IEntity<TIdentity>
    where TIdentity : IEquatable<TIdentity>
{
    Task AddAsync(TEntity entity, CancellationToken cancellationToken);

    Task<TEntity> GetAsync(TIdentity id, CancellationToken cancellationToken);

    Task<TEntity?> GetOrDefaultAsync(TIdentity id, CancellationToken cancellationToken);

    Task RemoveAsync(TEntity entity, CancellationToken cancellationToken);

    Task UpdateAsync(TEntity entity, CancellationToken cancellationToken);
}
