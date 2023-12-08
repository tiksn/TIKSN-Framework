using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace TIKSN.Data.Cache.Memory;

public class QueryRepositoryMemoryCacheDecorator<TEntity, TIdentity>
    : RepositoryMemoryCacheDecorator<TEntity, TIdentity>
    , IQueryRepository<TEntity, TIdentity>
    where TEntity : IEntity<TIdentity>
    where TIdentity : IEquatable<TIdentity>
{
    public QueryRepositoryMemoryCacheDecorator(
        IQueryRepository<TEntity, TIdentity> queryRepository,
        IRepository<TEntity> repository,
        IMemoryCache memoryCache,
        IOptions<MemoryCacheDecoratorOptions> genericOptions,
        IOptions<MemoryCacheDecoratorOptions<TEntity>> specificOptions)
        : base(repository, memoryCache, genericOptions, specificOptions) =>
        this.QueryRepository = queryRepository;

    protected IQueryRepository<TEntity, TIdentity> QueryRepository { get; }

    public Task<bool> ExistsAsync(TIdentity id, CancellationToken cancellationToken)
    {
        var cacheKey = Tuple.Create(EntityType, CacheKeyKind.Query, id);

        return this.GetFromMemoryCacheAsync(cacheKey,
            () => this.QueryRepository.ExistsAsync(id, cancellationToken));
    }

    public async Task<TEntity> GetAsync(TIdentity id, CancellationToken cancellationToken)
    {
        var cacheKey = Tuple.Create(EntityType, CacheKeyKind.Entity, id);

        return await this.GetFromMemoryCacheAsync(cacheKey,
            () => this.QueryRepository.GetAsync(id, cancellationToken)).ConfigureAwait(false)
            ?? throw new EntityNotFoundException("Result retrieved from cache or from original source is null.");
    }

    public Task<TEntity> GetOrDefaultAsync(TIdentity id, CancellationToken cancellationToken)
    {
        var cacheKey = Tuple.Create(EntityType, CacheKeyKind.Entity, id);

        return this.GetFromMemoryCacheAsync(cacheKey, () => this.QueryRepository.GetAsync(id, cancellationToken));
    }

    public async Task<IEnumerable<TEntity>>
        ListAsync(IEnumerable<TIdentity> ids, CancellationToken cancellationToken) =>
        await BatchOperationHelper.BatchOperationAsync(ids, cancellationToken, this.GetAsync).ConfigureAwait(false);

    public Task<PageResult<TEntity>> PageAsync(
        PageQuery pageQuery,
        CancellationToken cancellationToken)
    {
        var cacheKey = Tuple.Create(EntityType, CacheKeyKind.Query, pageQuery);

        return this.GetFromMemoryCacheAsync(cacheKey,
            () => this.QueryRepository.PageAsync(pageQuery, cancellationToken));
    }

    protected Task<IEnumerable<TEntity>> CreateMemoryCacheQueryAsync(
        ICacheEntry cacheEntry,
        Func<Task<IEnumerable<TEntity>>> queryFromSource)
    {
        ArgumentNullException.ThrowIfNull(cacheEntry);
        ArgumentNullException.ThrowIfNull(queryFromSource);

        this.SpecifyOptions(cacheEntry);

        return queryFromSource();
    }

    protected Task<IEnumerable<TEntity>> QueryFromMemoryCacheAsync(
        Func<Task<IEnumerable<TEntity>>> queryFromSource)
    {
        var cacheKey = Tuple.Create(EntityType, CacheKeyKind.Query);

        return this.QueryFromMemoryCacheAsync(cacheKey, queryFromSource);
    }

    protected Task<IEnumerable<TEntity>> QueryFromMemoryCacheAsync(object cacheKey,
        Func<Task<IEnumerable<TEntity>>> queryFromSource) =>
        this.MemoryCache.GetOrCreateAsync(cacheKey, x => this.CreateMemoryCacheQueryAsync(x, queryFromSource));
}
