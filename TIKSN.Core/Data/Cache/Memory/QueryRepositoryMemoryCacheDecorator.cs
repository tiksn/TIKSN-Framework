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

        public Task<bool> ExistsAsync(TIdentity id, CancellationToken cancellationToken)
        {
            var cacheKey = Tuple.Create(entityType, CacheKeyKind.Query, id);

            return GetQueryFromMemoryCacheAsync(cacheKey, () => _queryRepository.ExistsAsync(id, cancellationToken));
        }

        public async Task<TEntity> GetAsync(TIdentity id, CancellationToken cancellationToken)
        {
            var cacheKey = Tuple.Create(entityType, CacheKeyKind.Entity, id);

            var result = await GetFromMemoryCacheAsync(cacheKey, () => _queryRepository.GetAsync(id, cancellationToken));

            if (result == null)
            {
                throw new NullReferenceException("Result retrieved from cache or from original source is null.");
            }

            return result;
        }

        public Task<TEntity> GetOrDefaultAsync(TIdentity id, CancellationToken cancellationToken)
        {
            var cacheKey = Tuple.Create(entityType, CacheKeyKind.Entity, id);

            return GetFromMemoryCacheAsync(cacheKey, () => _queryRepository.GetAsync(id, cancellationToken));
        }

        protected Task<IEnumerable<TEntity>> CreateMemoryCacheQueryAsync(ICacheEntry cacheEntry, Func<Task<IEnumerable<TEntity>>> queryFromSource)
        {
            SpecifyOptions(cacheEntry);

            return queryFromSource();
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
    }
}