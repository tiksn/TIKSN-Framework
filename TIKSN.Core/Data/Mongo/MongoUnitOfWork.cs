using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;

namespace TIKSN.Data.Mongo
{
    public class MongoUnitOfWork : UnitOfWorkBase
    {
        protected readonly IClientSessionHandle _clientSessionHandle;
        private readonly IServiceScope _serviceScope;

        public MongoUnitOfWork(IClientSessionHandle clientSessionHandle, IServiceScope serviceScope)
        {
            this._clientSessionHandle =
                clientSessionHandle ?? throw new ArgumentNullException(nameof(clientSessionHandle));
            this._serviceScope = serviceScope ?? throw new ArgumentNullException(nameof(serviceScope));
        }

        public override Task CompleteAsync(CancellationToken cancellationToken) =>
            this._clientSessionHandle.CommitTransactionAsync(cancellationToken);

        public override Task DiscardAsync(CancellationToken cancellationToken) =>
            this._clientSessionHandle.AbortTransactionAsync(cancellationToken);

        public override void Dispose()
        {
            this._serviceScope?.Dispose();

            base.Dispose();
        }

        public override IServiceProvider Services => this._serviceScope.ServiceProvider;

        protected override bool IsDirty() => this._clientSessionHandle.WrappedCoreSession.IsDirty;
    }
}
