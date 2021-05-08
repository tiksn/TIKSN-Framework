using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;

namespace TIKSN.Data.Mongo
{
    public class MongoUnitOfWork : UnitOfWorkBase, IMongoUnitOfWork
    {
        protected readonly IClientSessionHandle _clientSessionHandle;
        private readonly IServiceScope _serviceScope;

        public MongoUnitOfWork(IClientSessionHandle clientSessionHandle, IServiceScope serviceScope)
        {
            _clientSessionHandle = clientSessionHandle ?? throw new ArgumentNullException(nameof(clientSessionHandle));
            _serviceScope = serviceScope ?? throw new ArgumentNullException(nameof(serviceScope));
        }

        public override Task CompleteAsync(CancellationToken cancellationToken)
        {
            return _clientSessionHandle.CommitTransactionAsync(cancellationToken);
        }

        public override Task DiscardAsync(CancellationToken cancellationToken)
        {
            return _clientSessionHandle.AbortTransactionAsync(cancellationToken);
        }

        protected override bool IsDirty()
        {
            return _clientSessionHandle.WrappedCoreSession.IsDirty;
        }

        public override void Dispose()
        {
            _serviceScope?.Dispose();
            
            base.Dispose();
        }

        public IServiceProvider Services => _serviceScope.ServiceProvider;
    }
}