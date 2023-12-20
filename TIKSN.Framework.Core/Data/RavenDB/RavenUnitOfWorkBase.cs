using Raven.Client.Documents;
using Raven.Client.Documents.Session;

namespace TIKSN.Data.RavenDB;

public abstract class RavenUnitOfWorkBase : UnitOfWorkBase
{
    protected RavenUnitOfWorkBase(IDocumentStore store)
    {
        ArgumentNullException.ThrowIfNull(store);

        this.Session = store.OpenAsyncSession();
    }

    protected IAsyncDocumentSession Session { get; }

    public override async Task CompleteAsync(CancellationToken cancellationToken)
    {
        await this.Session.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        this.Session.Advanced.Clear();
    }

    public override void Dispose()
    {
        this.Session.Dispose();

        base.Dispose();
    }

    protected override bool IsDirty() => false;

    //return _session.Advanced.HasChanges;
}
