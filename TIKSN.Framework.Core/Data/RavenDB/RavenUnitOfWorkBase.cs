using Raven.Client.Documents;
using Raven.Client.Documents.Session;

namespace TIKSN.Data.RavenDB;

public abstract class RavenUnitOfWorkBase : UnitOfWorkBase
{
    protected readonly IAsyncDocumentSession _session;

    protected RavenUnitOfWorkBase(IDocumentStore store)
    {
        ArgumentNullException.ThrowIfNull(store);

        this._session = store.OpenAsyncSession();
    }

    public override async Task CompleteAsync(CancellationToken cancellationToken)
    {
        await this._session.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        this._session.Advanced.Clear();
    }

    public override void Dispose()
    {
        this._session.Dispose();

        base.Dispose();
    }

    protected override bool IsDirty() => false;
    //return _session.Advanced.HasChanges;
}
