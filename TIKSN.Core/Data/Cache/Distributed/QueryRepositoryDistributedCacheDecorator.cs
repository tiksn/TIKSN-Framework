using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using TIKSN.Serialization;

namespace TIKSN.Data.Cache.Distributed;

public class QueryRepositoryDistributedCacheDecorator<TEntity, TIdentity>
    : RepositoryDistributedCacheDecorator<TEntity, TIdentity>
    , IQueryRepository<TEntity, TIdentity>
    where TEntity : IEntity<TIdentity>
    where TIdentity : IEquatable<TIdentity>
{
    protected readonly IQueryRepository<TEntity, TIdentity> queryRepository;

    public QueryRepositoryDistributedCacheDecorator(IQueryRepository<TEntity, TIdentity> queryRepository,
        IRepository<TEntity> repository,
        IDistributedCache distributedCache,
        ISerializer<byte[]> serializer,
        IDeserializer<byte[]> deserializer,
        IOptions<DistributedCacheDecoratorOptions> genericOptions,
        IOptions<DistributedCacheDecoratorOptions<TEntity>> specificOptions)
        : base(repository, distributedCache, serializer, deserializer, genericOptions, specificOptions) =>
        this.queryRepository = queryRepository;

    public async Task<bool> ExistsAsync(TIdentity id, CancellationToken cancellationToken)
    {
        var entity = await this.GetOrDefaultAsync(id, cancellationToken).ConfigureAwait(false);
        return entity is not null;
    }

    public Task<TEntity> GetAsync(TIdentity id, CancellationToken cancellationToken)
    {
        var cacheKey = Tuple.Create(entityType, CacheKeyKind.Entity, id).ToString();

        return this.GetFromDistributedCacheAsync(cacheKey, cancellationToken,
            () => this.queryRepository.GetAsync(id, cancellationToken))
            ?? throw new EntityNotFoundException("Result retrieved from cache or from original source is null.");
    }

    public Task<TEntity> GetOrDefaultAsync(TIdentity id, CancellationToken cancellationToken)
    {
        var cacheKey = Tuple.Create(entityType, CacheKeyKind.Entity, id).ToString();

        return this.GetFromDistributedCacheAsync(cacheKey, cancellationToken,
            () => this.queryRepository.GetAsync(id, cancellationToken));
    }

    public async Task<IEnumerable<TEntity>>
        ListAsync(IEnumerable<TIdentity> ids, CancellationToken cancellationToken) =>
        await BatchOperationHelper.BatchOperationAsync(ids, cancellationToken, this.GetAsync).ConfigureAwait(false);

    public Task<PageResult<TEntity>> PageAsync(
        PageQuery pageQuery,
        CancellationToken cancellationToken)
    {
        if (pageQuery is null)
        {
            throw new ArgumentNullException(nameof(pageQuery));
        }

        var cacheKey = Tuple.Create(
            entityType,
            CacheKeyKind.Query,
            pageQuery.Page.Number,
            pageQuery.Page.Size,
            pageQuery.EstimateTotalItems).ToString();

        return this.GetFromDistributedCacheAsync(
            cacheKey,
            cancellationToken,
            () => this.queryRepository.PageAsync(pageQuery, cancellationToken));
    }
}
