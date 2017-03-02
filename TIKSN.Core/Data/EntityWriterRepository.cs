using Microsoft.EntityFrameworkCore;

namespace TIKSN.Data
{
	public sealed class EntityWriterRepository<TContext, TEntity> : EntityRepositoryBase<TEntity>
		where TContext : DbContext
		where TEntity : class, new()
	{
		public EntityWriterRepository(TContext context) : base(context)
		{
		}
	}
}