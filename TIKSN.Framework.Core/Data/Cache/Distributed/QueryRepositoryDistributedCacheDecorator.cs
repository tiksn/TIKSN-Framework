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
    public QueryRepositoryDistributedCacheDecorator(IQueryRepository<TEntity, TIdentity> queryRepository,
        IRepository<TEntity> repository,
        IDistributedCache distributedCache,
        ISerializer<byte[]> serializer,
        IDeserializer<byte[]> deserializer,
        IOptions<DistributedCacheDecoratorOptions> genericOptions,
        IOptions<DistributedCacheDecoratorOptions<TEntity>> specificOptions)
        : base(repository, distributedCache, serializer, deserializer, genericOptions, specificOptions) =>
        this.QueryRepository = queryRepository;

    protected IQueryRepository<TEntity, TIdentity> QueryRepository { get; }

    public async Task<bool> ExistsAsync(TIdentity id, CancellationToken cancellationToken)
    {
        var entity = await this.GetOrDefaultAsync(id, cancellationToken).ConfigureAwait(false);
        return entity is not null;
    }

    public Task<TEntity> GetAsync(TIdentity id, CancellationToken cancellationToken)
    {
        var cacheKey = Tuple.Create(EntityType, CacheKeyKind.Entity, id).ToString();

        return this.GetFromDistributedCacheAsync(cacheKey, () => this.QueryRepository.GetAsync(id, cancellationToken),
            cancellationToken)
            ?? throw new EntityNotFoundException("Result retrieved from cache or from original source is null.");
    }

    public Task<TEntity> GetOrDefaultAsync(TIdentity id, CancellationToken cancellationToken)
    {
        var cacheKey = Tuple.Create(EntityType, CacheKeyKind.Entity, id).ToString();

        return this.GetFromDistributedCacheAsync(cacheKey, () => this.QueryRepository.GetAsync(id, cancellationToken),
            cancellationToken);
    }

    public async Task<IEnumerable<TEntity>>
        ListAsync(IEnumerable<TIdentity> ids, CancellationToken cancellationToken) =>
        await BatchOperationHelper.BatchOperationAsync(ids, this.GetAsync, cancellationToken).ConfigureAwait(false);

    public Task<PageResult<TEntity>> PageAsync(
        PageQuery pageQuery,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(pageQuery);

        var cacheKey = Tuple.Create(
            EntityType,
            CacheKeyKind.Query,
            pageQuery.Page.Number,
            pageQuery.Page.Size,
            pageQuery.EstimateTotalItems).ToString();

        return this.GetFromDistributedCacheAsync(
            cacheKey,
            () => this.QueryRepository.PageAsync(pageQuery, cancellationToken),
            cancellationToken);
    }
}
