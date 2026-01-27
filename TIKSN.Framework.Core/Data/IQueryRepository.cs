namespace TIKSN.Data;

public interface IQueryRepository<TEntity, TIdentity>
    where TEntity : IEntity<TIdentity>
    where TIdentity : IEquatable<TIdentity>
{
    public Task<bool> ExistsAsync(TIdentity id, CancellationToken cancellationToken);

    public Task<TEntity> GetAsync(TIdentity id, CancellationToken cancellationToken);

    public Task<TEntity?> GetOrDefaultAsync(TIdentity id, CancellationToken cancellationToken);

    public Task<IReadOnlyList<TEntity>> ListAsync(IEnumerable<TIdentity> ids, CancellationToken cancellationToken);

    public Task<PageResult<TEntity>> PageAsync(PageQuery pageQuery, CancellationToken cancellationToken);
}
