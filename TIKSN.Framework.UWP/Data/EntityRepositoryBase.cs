using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace TIKSN.Data
{
	public abstract class EntityRepositoryBase<T> : IRepository<T>
		where T : class, new()
	{
		protected readonly DbContext dbContext;

		protected EntityRepositoryBase(DbContext dbContext)
		{
			this.dbContext = dbContext;
		}

		public Task AddAsync(T entity, CancellationToken cancellationToken = default(CancellationToken))
		{
			dbContext.Add<T>(entity);

			return Task.FromResult<object>(null);
		}

		public Task AddRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default(CancellationToken))
		{
			dbContext.AddRange(entities);

			return Task.FromResult<object>(null);
		}

		public Task RemoveAsync(T entity, CancellationToken cancellationToken = default(CancellationToken))
		{
			dbContext.Entry<T>(entity).State = EntityState.Deleted;

			return Task.FromResult<object>(null);
		}

		public Task RemoveRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default(CancellationToken))
		{
			foreach (var entity in entities)
			{
				dbContext.Entry<T>(entity).State = EntityState.Deleted;
			}

			return Task.FromResult<object>(null);
		}

		public Task UpdateAsync(T entity, CancellationToken cancellationToken = default(CancellationToken))
		{
			dbContext.Entry<T>(entity).State = EntityState.Modified;

			return Task.FromResult<object>(null);
		}

		public Task UpdateRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default(CancellationToken))
		{
			foreach (var entity in entities)
			{
				dbContext.Entry<T>(entity).State = EntityState.Modified;
			}

			return Task.FromResult<object>(null);
		}

		protected async Task<IEnumerable<T>> SearchAsync(Func<DbSet<T>, IQueryable<T>> filter, CancellationToken cancellationToken = default(CancellationToken))
		{
			var tableDataSet = dbContext.Set<T>();
			var query = filter(tableDataSet);

			IEnumerable<T> matchedEntities = await query.ToListAsync(cancellationToken);

			return matchedEntities;
		}
	}
}