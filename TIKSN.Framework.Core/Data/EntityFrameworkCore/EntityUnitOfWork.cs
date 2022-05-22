using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace TIKSN.Data.EntityFrameworkCore
{
    public class EntityUnitOfWork : UnitOfWorkBase
    {
        private readonly DbContext[] _dbContexts;

        public EntityUnitOfWork(DbContext[] dbContexts) => this._dbContexts = dbContexts;

        public override async Task CompleteAsync(CancellationToken cancellationToken)
        {
            var tasks = this._dbContexts.Select(dbContext => dbContext.SaveChangesAsync(cancellationToken)).ToArray();

            _ = await Task.WhenAll(tasks).ConfigureAwait(false);
        }

        public override Task DiscardAsync(CancellationToken cancellationToken)
        {
            _ = this._dbContexts.Do(dbContext => dbContext.ChangeTracker.Clear());

            return Task.CompletedTask;
        }

        protected override bool IsDirty() => this._dbContexts.Any(dbContext => dbContext.ChangeTracker.HasChanges());
    }
}
