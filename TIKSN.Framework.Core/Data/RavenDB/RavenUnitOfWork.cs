using Microsoft.Extensions.DependencyInjection;
using Raven.Client.Documents;
using Raven.Client.Documents.Session;

namespace TIKSN.Data.RavenDB;

public class RavenUnitOfWork : UnitOfWorkBase
{
    private readonly AsyncServiceScope serviceScope;

    public RavenUnitOfWork(
        IDocumentStore store,
        AsyncServiceScope serviceScope)
    {
        ArgumentNullException.ThrowIfNull(store);

        var sessionStore = serviceScope.ServiceProvider.GetRequiredService<IRavenSessionStore>();
        var session = store.OpenAsyncSession();
        sessionStore.SetSession(session);
        this.Session = session;
        this.serviceScope = serviceScope;
    }

    public override IServiceProvider Services => this.serviceScope.ServiceProvider;

    protected IAsyncDocumentSession Session { get; }

    public override async Task CompleteAsync(CancellationToken cancellationToken)
    {
        await this.Session.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        this.Session.Advanced.Clear();
    }

    public override async Task DiscardAsync(CancellationToken cancellationToken)
    {
        this.Session.Advanced.Clear();

        this.Session.Dispose();
        await this.serviceScope.DisposeAsync().ConfigureAwait(false);
    }

    protected override bool IsDirty() => this.Session.Advanced.HasChanges;
}
