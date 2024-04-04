using Raven.Client.Documents;
using Raven.Client.Documents.Linq;

namespace TIKSN.Data.RavenDB;

public class RavenRepository<TEntity, TIdentity> : IRavenRepository<TEntity, TIdentity>
    where TEntity : IEntity<TIdentity>
    where TIdentity : IEquatable<TIdentity>
{
    private readonly IRavenSessionProvider sessionProvider;

    public RavenRepository(IRavenSessionProvider sessionProvider)
        => this.sessionProvider = sessionProvider ?? throw new ArgumentNullException(nameof(sessionProvider));

    public Task AddAsync(TEntity entity, CancellationToken cancellationToken) =>
        this.sessionProvider.Session.StoreAsync(entity, entity.ID.ToString(), cancellationToken);

    public Task AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken) =>
        BatchOperationHelper.BatchOperationAsync(entities, this.AddAsync, cancellationToken);

    public Task<bool> ExistsAsync(TIdentity id, CancellationToken cancellationToken) =>
        this.sessionProvider.Session.Advanced.ExistsAsync(id.ToString(), cancellationToken);

    public async Task<TEntity> GetAsync(TIdentity id, CancellationToken cancellationToken)
        => await this.GetOrDefaultAsync(id, cancellationToken).ConfigureAwait(false)
            ?? throw new EntityNotFoundException($"Entity with ID '{id}' is not found.");

    public Task<TEntity> GetOrDefaultAsync(TIdentity id, CancellationToken cancellationToken) =>
        this.sessionProvider.Session.LoadAsync<TEntity>(id.ToString(), cancellationToken);

    public async Task<IEnumerable<TEntity>> ListAsync(IEnumerable<TIdentity> ids,
        CancellationToken cancellationToken) =>
        await this.sessionProvider.Session.Query<TEntity>()
            .Where(entity => ids.Contains(entity.ID))
            .ToListAsync(cancellationToken).ConfigureAwait(false);

    public Task<PageResult<TEntity>> PageAsync(
        PageQuery pageQuery,
        CancellationToken cancellationToken)
        => PageAsync(
            this.sessionProvider.Session.Query<TEntity>(),
            pageQuery,
            cancellationToken);

    public Task RemoveAsync(TEntity entity, CancellationToken cancellationToken)
    {
        this.sessionProvider.Session.Delete(entity.ID.ToString());

        return Task.CompletedTask;
    }

    public Task RemoveRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken) =>
        BatchOperationHelper.BatchOperationAsync(entities, this.RemoveAsync, cancellationToken);

    public IAsyncEnumerable<TEntity> StreamAllAsync(CancellationToken cancellationToken) =>
        this.sessionProvider.Session.Query<TEntity>()
            .ToAsyncEnumerable();

    public Task UpdateAsync(TEntity entity, CancellationToken cancellationToken) =>
        this.sessionProvider.Session.StoreAsync(entity, entity.ID.ToString(), cancellationToken);

    public Task UpdateRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken) =>
        BatchOperationHelper.BatchOperationAsync(entities, this.UpdateAsync, cancellationToken);

    protected static Task<PageResult<TEntity>> PageAsync(
        IRavenQueryable<TEntity> query,
        PageQuery pageQuery,
        CancellationToken cancellationToken)
        => PaginationQueryableHelper.PageAsync(
            query,
            pageQuery,
            static (q, ct) => q.ToListAsync(ct),
            static (q, ct) => q.LongCountAsync(ct),
            cancellationToken);
}
