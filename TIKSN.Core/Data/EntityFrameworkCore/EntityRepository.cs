using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace TIKSN.Data.EntityFrameworkCore
{
	public class EntityRepository<TContext, TEntity> : IRepository<TEntity>
		where TContext : DbContext
		where TEntity : class, new()
	{
		protected readonly TContext dbContext;

		public EntityRepository(TContext dbContext)
		{
			this.dbContext = dbContext;
		}

		public Task AddAsync(TEntity entity, CancellationToken cancellationToken = default(CancellationToken))
		{
			dbContext.Add(entity);

			return Task.FromResult<object>(null);
		}

		public Task AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default(CancellationToken))
		{
			dbContext.AddRange(entities);

			return Task.FromResult<object>(null);
		}

		public Task RemoveAsync(TEntity entity, CancellationToken cancellationToken = default(CancellationToken))
		{
			dbContext.Entry(entity).State = EntityState.Deleted;

			return Task.FromResult<object>(null);
		}

		public Task RemoveRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default(CancellationToken))
		{
			foreach (var entity in entities)
				dbContext.Entry(entity).State = EntityState.Deleted;

			return Task.FromResult<object>(null);
		}

		public Task UpdateAsync(TEntity entity, CancellationToken cancellationToken = default(CancellationToken))
		{
			dbContext.Entry(entity).State = EntityState.Modified;

			return Task.FromResult<object>(null);
		}

		public Task UpdateRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default(CancellationToken))
		{
			foreach (var entity in entities)
				dbContext.Entry(entity).State = EntityState.Modified;

			return Task.FromResult<object>(null);
		}
	}
}