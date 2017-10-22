using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace TIKSN.Data.Cache.Memory
{
    public class QueryRepositoryMemoryCacheDecorator<TEntity, TIdentity>
    : RepositoryMemoryCacheDecorator<TEntity, TIdentity>, IQueryRepository<TEntity, TIdentity>
        where TEntity : IEntity<TIdentity>
        where TIdentity : IEquatable<TIdentity>
    {
        protected readonly IQueryRepository<TEntity, TIdentity> _queryRepository;

        public QueryRepositoryMemoryCacheDecorator(IQueryRepository<TEntity, TIdentity> queryRepository,
            IRepository<TEntity> repository,
            IMemoryCache memoryCache,
            IOptions<MemoryCacheDecoratorOptions> genericOptions,
            IOptions<MemoryCacheDecoratorOptions<TEntity>> specificOptions)
            : base(repository, memoryCache, genericOptions, specificOptions)
        {
            _queryRepository = queryRepository;
        }

        public Task<TEntity> GetAsync(TIdentity id, CancellationToken cancellationToken)
        {
            var cacheKey = Tuple.Create(entityType, CacheKeyKind.Entity, id);

            return GetFromMemoryCacheAsync(cacheKey, () => _queryRepository.GetAsync(id, cancellationToken));
        }

        protected Task<IEnumerable<TEntity>> QueryFromMemoryCacheAsync(Func<Task<IEnumerable<TEntity>>> queryFromSource, CancellationToken cancellationToken)
        {
            var cacheKey = Tuple.Create(entityType, CacheKeyKind.Query);

            return QueryFromMemoryCacheAsync(cacheKey, queryFromSource, cancellationToken);
        }

        protected Task<IEnumerable<TEntity>> QueryFromMemoryCacheAsync(object cacheKey, Func<Task<IEnumerable<TEntity>>> queryFromSource, CancellationToken cancellationToken)
        {
            return _memoryCache.GetOrCreateAsync(cacheKey, x => CreateMemoryCacheQueryAsync(x, queryFromSource));
        }

        protected Task<IEnumerable<TEntity>> CreateMemoryCacheQueryAsync(ICacheEntry cacheEntry, Func<Task<IEnumerable<TEntity>>> queryFromSource)
        {
            SpecifyOptions(cacheEntry);

            return queryFromSource();
        }
    }
}