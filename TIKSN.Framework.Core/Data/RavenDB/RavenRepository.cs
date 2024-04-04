using Raven.Client;
using Raven.Client.Documents;
using Raven.Client.Documents.Linq;

namespace TIKSN.Data.RavenDB;

public class RavenRepository<TEntity, TIdentity> : IRavenRepository<TEntity, TIdentity>
    where TEntity : IEntity<TIdentity>
    where TIdentity : IEquatable<TIdentity>
{
    public RavenRepository(IRavenSessionProvider sessionProvider, string collectionName)
    {
        if (string.IsNullOrWhiteSpace(collectionName))
        {
            throw new ArgumentException($"'{nameof(collectionName)}' cannot be null or whitespace.", nameof(collectionName));
        }

        this.SessionProvider = sessionProvider ?? throw new ArgumentNullException(nameof(sessionProvider));
        this.CollectionName = collectionName;
    }

    public string CollectionName { get; }

    public IRavenSessionProvider SessionProvider { get; }

    public async Task AddAsync(TEntity entity, CancellationToken cancellationToken)
    {
        await this.SessionProvider.Session.StoreAsync(entity, this.CreateDocumentId(entity.ID), cancellationToken).ConfigureAwait(false);
        this.SessionProvider.Session.Advanced.GetMetadataFor(entity)[Constants.Documents.Metadata.Collection] = this.CollectionName;
    }

    public Task AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken) =>
        BatchOperationHelper.BatchOperationAsync(entities, this.AddAsync, cancellationToken);

    public Task<bool> ExistsAsync(TIdentity id, CancellationToken cancellationToken) =>
        this.SessionProvider.Session.Advanced.ExistsAsync(this.CreateDocumentId(id), cancellationToken);

    public async Task<TEntity> GetAsync(TIdentity id, CancellationToken cancellationToken)
        => await this.GetOrDefaultAsync(id, cancellationToken).ConfigureAwait(false)
            ?? throw new EntityNotFoundException($"Entity with ID '{id}' is not found.");

    public Task<TEntity> GetOrDefaultAsync(TIdentity id, CancellationToken cancellationToken) =>
        this.SessionProvider.Session.LoadAsync<TEntity>(this.CreateDocumentId(id), cancellationToken);

    public async Task<IEnumerable<TEntity>> ListAsync(IEnumerable<TIdentity> ids,
        CancellationToken cancellationToken) =>
        await this.SessionProvider.Session.Query<TEntity>(collectionName: this.CollectionName)
            .Where(entity => ids.Contains(entity.ID))
            .ToListAsync(cancellationToken).ConfigureAwait(false);

    public Task<PageResult<TEntity>> PageAsync(
        PageQuery pageQuery,
        CancellationToken cancellationToken)
        => PageAsync(
            this.SessionProvider.Session.Query<TEntity>(collectionName: this.CollectionName),
            pageQuery,
            cancellationToken);

    public Task RemoveAsync(TEntity entity, CancellationToken cancellationToken)
    {
        this.SessionProvider.Session.Delete(this.CreateDocumentId(entity.ID));

        return Task.CompletedTask;
    }

    public Task RemoveRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken) =>
        BatchOperationHelper.BatchOperationAsync(entities, this.RemoveAsync, cancellationToken);

    public IAsyncEnumerable<TEntity> StreamAllAsync(CancellationToken cancellationToken) =>
        this.SessionProvider.Session.Query<TEntity>(collectionName: this.CollectionName)
            .ToAsyncEnumerable();

    public Task UpdateAsync(TEntity entity, CancellationToken cancellationToken) =>
        this.SessionProvider.Session.StoreAsync(entity, this.CreateDocumentId(entity.ID), cancellationToken);

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

    protected string CreateDocumentId(TIdentity id) => $"{this.CollectionName}/{id}";
}
