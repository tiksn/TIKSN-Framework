using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

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
            : base(repository, memoryCache, genericOptions, specificOptions) =>
            this._queryRepository = queryRepository;

        public Task<bool> ExistsAsync(TIdentity id, CancellationToken cancellationToken)
        {
            var cacheKey = Tuple.Create(entityType, CacheKeyKind.Query, id);

            return this.GetQueryFromMemoryCacheAsync(cacheKey,
                () => this._queryRepository.ExistsAsync(id, cancellationToken));
        }

        public async Task<TEntity> GetAsync(TIdentity id, CancellationToken cancellationToken)
        {
            var cacheKey = Tuple.Create(entityType, CacheKeyKind.Entity, id);

            var result = await this.GetFromMemoryCacheAsync(cacheKey,
                () => this._queryRepository.GetAsync(id, cancellationToken)).ConfigureAwait(false);

            if (result == null)
            {
                throw new NullReferenceException("Result retrieved from cache or from original source is null.");
            }

            return result;
        }

        public Task<TEntity> GetOrDefaultAsync(TIdentity id, CancellationToken cancellationToken)
        {
            var cacheKey = Tuple.Create(entityType, CacheKeyKind.Entity, id);

            return this.GetFromMemoryCacheAsync(cacheKey, () => this._queryRepository.GetAsync(id, cancellationToken));
        }

        public async Task<IEnumerable<TEntity>>
            ListAsync(IEnumerable<TIdentity> ids, CancellationToken cancellationToken) =>
            await BatchOperationHelper.BatchOperationAsync(ids, cancellationToken, (id, ct) => this.GetAsync(id, ct)).ConfigureAwait(false);

        protected Task<IEnumerable<TEntity>> CreateMemoryCacheQueryAsync(ICacheEntry cacheEntry,
            Func<Task<IEnumerable<TEntity>>> queryFromSource)
        {
            this.SpecifyOptions(cacheEntry);

            return queryFromSource();
        }

        protected Task<IEnumerable<TEntity>> QueryFromMemoryCacheAsync(Func<Task<IEnumerable<TEntity>>> queryFromSource)
        {
            var cacheKey = Tuple.Create(entityType, CacheKeyKind.Query);

            return this.QueryFromMemoryCacheAsync(cacheKey, queryFromSource);
        }

        protected Task<IEnumerable<TEntity>> QueryFromMemoryCacheAsync(object cacheKey,
            Func<Task<IEnumerable<TEntity>>> queryFromSource) =>
            this._memoryCache.GetOrCreateAsync(cacheKey, x => this.CreateMemoryCacheQueryAsync(x, queryFromSource));
    }
}
