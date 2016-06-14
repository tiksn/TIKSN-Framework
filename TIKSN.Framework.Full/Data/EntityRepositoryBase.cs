//using Microsoft.EntityFrameworkCore;
//using System.Collections.Generic;
//using System.Threading;
//using System.Threading.Tasks;

//namespace TIKSN.Data
//{
//	public abstract class EntityRepositoryBase<T> : IRepository<T>
//		where T : class, new()
//	{
//		protected readonly DbContext dbContext;

//		protected EntityRepositoryBase(DbContext dbContext)
//		{
//			this.dbContext = dbContext;
//		}

//		public async Task AddAsync(T entity, CancellationToken cancellationToken = default(CancellationToken))
//		{
//			await dbContext.AddAsync<T>(entity, cancellationToken);
//		}

//		public async Task AddRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default(CancellationToken))
//		{
//			foreach (var entity in entities)
//			{
//				await dbContext.AddAsync<T>(entity, cancellationToken);
//			}
//		}

//		public Task RemoveAsync(T entity, CancellationToken cancellationToken = default(CancellationToken))
//		{
//			dbContext.Delete<T>(entity);

//			return Task.FromResult<object>(null);
//		}

//		public Task RemoveRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default(CancellationToken))
//		{
//			foreach (var entity in entities)
//			{
//				dbContext.Delete<T>(entity);
//			}

//			return Task.FromResult<object>(null);
//		}

//		public async Task UpdateAsync(T entity, CancellationToken cancellationToken = default(CancellationToken))
//		{
//			await dbContext.UpdateAsync<T>(entity, cancellationToken);
//		}

//		public async Task UpdateRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default(CancellationToken))
//		{
//			foreach (var entity in entities)
//			{
//				await dbContext.UpdateAsync<T>(entity, cancellationToken);
//			}
//		}
//	}
//}
