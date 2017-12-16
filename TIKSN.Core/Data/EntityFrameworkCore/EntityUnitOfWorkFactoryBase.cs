using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;

namespace TIKSN.Data.EntityFrameworkCore
{
    public abstract class EntityUnitOfWorkFactoryBase : IUnitOfWorkFactory
    {
        private readonly IServiceProvider serviceProvider;
        protected readonly ILogger<EntityUnitOfWorkFactoryBase> _logger;
        private readonly ILogger<EntityUnitOfWork> _entityUnitOfWorkLogger;

        protected EntityUnitOfWorkFactoryBase(ILogger<EntityUnitOfWorkFactoryBase> logger, ILogger<EntityUnitOfWork> entityUnitOfWorkLogger, IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _entityUnitOfWorkLogger = entityUnitOfWorkLogger ?? throw new ArgumentNullException(nameof(entityUnitOfWorkLogger));
        }

        public IUnitOfWork Create()
        {
            _logger.LogDebug("Creating unit of work.");

            var dbContexts = GetContexts();

            _logger.LogDebug($"Created {dbContexts.Length} context(s).");

            return new EntityUnitOfWork(_entityUnitOfWorkLogger, dbContexts);
        }

        protected abstract DbContext[] GetContexts();

        protected TContext GetContext<TContext>() where TContext : DbContext
        {
            return serviceProvider.GetRequiredService<TContext>();
        }
    }
}