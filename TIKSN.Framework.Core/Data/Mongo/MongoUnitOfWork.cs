using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;

namespace TIKSN.Data.Mongo;

public class MongoUnitOfWork : UnitOfWorkBase
{
    private readonly IClientSessionHandle clientSessionHandle;
    private readonly AsyncServiceScope serviceScope;

    public MongoUnitOfWork(IClientSessionHandle clientSessionHandle, AsyncServiceScope serviceScope)
    {
        this.clientSessionHandle =
            clientSessionHandle ?? throw new ArgumentNullException(nameof(clientSessionHandle));
        this.serviceScope = serviceScope;
    }

    public override IServiceProvider Services => this.serviceScope.ServiceProvider;

    public override Task CompleteAsync(CancellationToken cancellationToken) =>
        this.clientSessionHandle.CommitTransactionAsync(cancellationToken);

    public override Task DiscardAsync(CancellationToken cancellationToken) =>
        this.clientSessionHandle.AbortTransactionAsync(cancellationToken);

    public override async ValueTask DisposeAsync()
    {
        await base.DisposeAsync().ConfigureAwait(false);
        await this.serviceScope.DisposeAsync().ConfigureAwait(false);
    }

    protected override bool IsDirty() => this.clientSessionHandle.WrappedCoreSession.IsDirty;
}
