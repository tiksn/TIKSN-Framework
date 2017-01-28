using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace TIKSN.Data
{
	public class EntityUnitOfWork : UnitOfWorkBase
	{
		private readonly DbContext dbContext;

		public EntityUnitOfWork(DbContext dbContext)
		{
			this.dbContext = dbContext;
		}

		public override Task CompleteAsync()
		{
			return dbContext.SaveChangesAsync();
		}

		protected override bool IsDirty()
		{
			return dbContext.ChangeTracker.HasChanges();
		}
	}
}