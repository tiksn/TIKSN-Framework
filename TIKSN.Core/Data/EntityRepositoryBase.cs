using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

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

		protected IQueryable<T> Entities => dbContext.Set<T>().AsNoTracking();

		public Task AddAsync(T entity, CancellationToken cancellationToken = default(CancellationToken))
		{
			dbContext.Add(entity);

			return Task.FromResult<object>(null);
		}

		public Task AddRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default(CancellationToken))
		{
			dbContext.AddRange(entities);

			return Task.FromResult<object>(null);
		}

		public Task RemoveAsync(T entity, CancellationToken cancellationToken = default(CancellationToken))
		{
			dbContext.Entry(entity).State = EntityState.Deleted;

			return Task.FromResult<object>(null);
		}

		public Task RemoveRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default(CancellationToken))
		{
			foreach (var entity in entities)
				dbContext.Entry(entity).State = EntityState.Deleted;

			return Task.FromResult<object>(null);
		}

		public Task UpdateAsync(T entity, CancellationToken cancellationToken = default(CancellationToken))
		{
			dbContext.Entry(entity).State = EntityState.Modified;

			return Task.FromResult<object>(null);
		}

		public Task UpdateRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default(CancellationToken))
		{
			foreach (var entity in entities)
				dbContext.Entry(entity).State = EntityState.Modified;

			return Task.FromResult<object>(null);
		}
	}
}