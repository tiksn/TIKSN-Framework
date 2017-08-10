using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;

namespace TIKSN.Data.EntityFrameworkCore
{
    public class EntityUnitOfWork : UnitOfWorkBase
    {
        private readonly DbContext dbContext;

        public EntityUnitOfWork(DbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public override Task CompleteAsync(CancellationToken cancellationToken)
        {
            return dbContext.SaveChangesAsync(cancellationToken);
        }

        protected override bool IsDirty()
        {
            return dbContext.ChangeTracker.HasChanges();
        }
    }
}