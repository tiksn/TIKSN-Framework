using LanguageExt;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Options;

namespace TIKSN.Data.Cache.Hybrid;

public class QueryRepositoryHybridCacheDecorator<TEntity, TIdentity>
    : RepositoryHybridCacheDecorator<TEntity, TIdentity>
    , IQueryRepository<TEntity, TIdentity>
    where TEntity : IEntity<TIdentity>
    where TIdentity : IEquatable<TIdentity>
{
    public QueryRepositoryHybridCacheDecorator(
        IQueryRepository<TEntity, TIdentity> queryRepository,
        IRepository<TEntity> repository,
        HybridCache hybridCache,
        IOptions<HybridCacheDecoratorOptions> genericOptions,
        IOptions<HybridCacheDecoratorOptions<TEntity>> specificOptions)
        : base(repository, hybridCache, genericOptions, specificOptions) =>
        this.QueryRepository = queryRepository;

    protected IQueryRepository<TEntity, TIdentity> QueryRepository { get; }

    public async Task<bool> ExistsAsync(TIdentity id, CancellationToken cancellationToken)
    {
        var cacheKey = Tuple.Create(EntityType, CacheKeyKind.Query, id).ToString();

        return await this.GetFromHybridCacheAsync(
            cacheKey,
            async ct => await this.QueryRepository.ExistsAsync(id, ct).ConfigureAwait(false),
            cancellationToken).ConfigureAwait(false);
    }

    public async Task<TEntity> GetAsync(TIdentity id, CancellationToken cancellationToken)
    {
        var cacheKey = Tuple.Create(EntityType, CacheKeyKind.Entity, id).ToString();

        return await this.GetFromHybridCacheAsync(
            cacheKey,
            async ct => await this.QueryRepository.GetAsync(id, ct).ConfigureAwait(false),
            cancellationToken).ConfigureAwait(false)
            ?? throw new EntityNotFoundException("Result retrieved from cache or from original source is null.");
    }

    public async Task<TEntity?> GetOrDefaultAsync(TIdentity id, CancellationToken cancellationToken)
    {
        var cacheKey = Tuple.Create(EntityType, CacheKeyKind.Entity, id).ToString();

        return await this.GetFromHybridCacheAsync(
            cacheKey,
            async ct => await this.QueryRepository.GetOrDefaultAsync(id, ct).ConfigureAwait(false),
            cancellationToken).ConfigureAwait(false);
    }

    public async Task<IReadOnlyList<TEntity>> ListAsync(
        IEnumerable<TIdentity> ids,
        CancellationToken cancellationToken)
        => await BatchOperationHelper.BatchOperationAsync(ids, this.GetAsync, cancellationToken).ConfigureAwait(false);

    public async Task<PageResult<TEntity>> PageAsync(
        PageQuery pageQuery,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(pageQuery);

        var cacheKey = Tuple.Create(
            EntityType,
            CacheKeyKind.Page,
            pageQuery.Page.Size,
            pageQuery.Page.Number,
            pageQuery.EstimateTotalItems).ToString();

        var result = await this.GetFromHybridCacheAsync(
            cacheKey,
            async ct => await this.QueryRepository.PageAsync(pageQuery, ct).ConfigureAwait(false),
            cancellationToken).ConfigureAwait(false);

        return result ?? new PageResult<TEntity>(
            pageQuery.Page,
            [],
            Option<long>.None);
    }
}
