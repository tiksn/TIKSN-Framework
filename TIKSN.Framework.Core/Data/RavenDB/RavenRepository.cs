using Raven.Client.Documents;
using Raven.Client.Documents.Linq;
using Raven.Client.Documents.Session;

namespace TIKSN.Data.RavenDB;

public class RavenRepository<TEntity, TIdentity> : IRepository<TEntity>, IQueryRepository<TEntity, TIdentity>,
    IStreamRepository<TEntity>
    where TEntity : IEntity<TIdentity>
    where TIdentity : IEquatable<TIdentity>
{
    private readonly IAsyncDocumentSession session;

    public RavenRepository(IAsyncDocumentSession session) =>
        this.session = session ?? throw new ArgumentNullException(nameof(session));

    public Task AddAsync(TEntity entity, CancellationToken cancellationToken) =>
        this.session.StoreAsync(entity, entity.ID.ToString(), cancellationToken);

    public Task AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken) =>
        BatchOperationHelper.BatchOperationAsync(entities, cancellationToken, this.AddAsync);

    public Task<bool> ExistsAsync(TIdentity id, CancellationToken cancellationToken) =>
        this.session.Advanced.ExistsAsync(id.ToString(), cancellationToken);

    public async Task<TEntity> GetAsync(TIdentity id, CancellationToken cancellationToken)
        => await this.GetOrDefaultAsync(id, cancellationToken).ConfigureAwait(false)
            ?? throw new EntityNotFoundException($"Entity with ID '{id}' is not found.");

    public Task<TEntity> GetOrDefaultAsync(TIdentity id, CancellationToken cancellationToken) =>
        this.session.LoadAsync<TEntity>(id.ToString(), cancellationToken);

    public async Task<IEnumerable<TEntity>> ListAsync(IEnumerable<TIdentity> ids,
        CancellationToken cancellationToken) =>
        await this.session.Query<TEntity>()
            .Where(entity => ids.Contains(entity.ID))
            .ToListAsync(cancellationToken).ConfigureAwait(false);

    public Task<PageResult<TEntity>> PageAsync(
        PageQuery pageQuery,
        CancellationToken cancellationToken)
        => this.PageAsync(
            this.session.Query<TEntity>(),
            pageQuery,
            cancellationToken);

    public Task RemoveAsync(TEntity entity, CancellationToken cancellationToken)
    {
        this.session.Delete(entity.ID.ToString());

        return Task.CompletedTask;
    }

    public Task RemoveRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken) =>
        BatchOperationHelper.BatchOperationAsync(entities, cancellationToken, this.RemoveAsync);

    public IAsyncEnumerable<TEntity> StreamAllAsync(CancellationToken cancellationToken) =>
        this.session.Query<TEntity>()
            .ToAsyncEnumerable();

    public Task UpdateAsync(TEntity entity, CancellationToken cancellationToken) =>
        this.session.StoreAsync(entity, entity.ID.ToString(), cancellationToken);

    public Task UpdateRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken) =>
        BatchOperationHelper.BatchOperationAsync(entities, cancellationToken, this.UpdateAsync);

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
