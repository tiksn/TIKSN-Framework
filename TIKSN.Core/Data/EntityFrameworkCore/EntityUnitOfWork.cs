using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace TIKSN.Data.EntityFrameworkCore
{
    public class EntityUnitOfWork : UnitOfWorkBase
    {
        private readonly DbContext[] _dbContexts;
        private readonly ILogger<EntityUnitOfWork> _logger;

        public EntityUnitOfWork(ILogger<EntityUnitOfWork> logger, DbContext[] dbContexts)
        {
            _dbContexts = dbContexts;
            _logger = logger ?? throw new System.ArgumentNullException(nameof(logger));
        }

        public override async Task CompleteAsync(CancellationToken cancellationToken)
        {
            _logger.LogDebug("EF unit of work completion started.");

            var tasks = _dbContexts.Select(dbContext => dbContext.SaveChangesAsync(cancellationToken)).ToArray();

            await Task.WhenAll(tasks);

            _logger.LogDebug("EF unit of work completion ended.");
        }

        protected override bool IsDirty()
        {
            var changes = new List<bool>();

            foreach (var dbContext in _dbContexts)
            {
                _logger.LogDebug($"AutoDetectChangesEnabled = {dbContext.ChangeTracker.AutoDetectChangesEnabled}");
                dbContext.ChangeTracker.DetectChanges();
                changes.Add(dbContext.ChangeTracker.HasChanges());
            }

            var hasChanges = changes.Any(change => change);

            _logger.LogDebug($"Has changes: {hasChanges}");

            return hasChanges;
        }
    }
}