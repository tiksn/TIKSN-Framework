using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace TIKSN.Data.Cache
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
			var cacheKey = Tuple.Create(entityType, MemoryCacheKeyKind.Entity, id);

			return GetFromMemoryCacheAsync(cacheKey, () => _queryRepository.GetAsync(id, cancellationToken));
		}
	}
}
