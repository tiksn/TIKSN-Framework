using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace TIKSN.Data.EntityFrameworkCore
{
    public class EntityUnitOfWork : UnitOfWorkBase
    {
        private readonly DbContext[] _dbContexts;

        public EntityUnitOfWork(DbContext[] dbContexts)
        {
            _dbContexts = dbContexts;
        }

        public override async Task CompleteAsync(CancellationToken cancellationToken)
        {
            var tasks = _dbContexts.Select(dbContext => dbContext.SaveChangesAsync(cancellationToken)).ToArray();

            await Task.WhenAll(tasks);
        }

        public override Task DiscardAsync(CancellationToken cancellationToken)
        {
            _dbContexts.Do(dbContext => dbContext.ChangeTracker.Clear());

            return Task.CompletedTask;
        }

        protected override bool IsDirty()
        {
            return _dbContexts.Any(dbContext => dbContext.ChangeTracker.HasChanges());
        }
    }
}