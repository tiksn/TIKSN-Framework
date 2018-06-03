using Raven.Client.Documents;
using Raven.Client.Documents.Session;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace TIKSN.Data.RavenDB
{
    public abstract class RavenUnitOfWorkBase : UnitOfWorkBase
    {
        protected readonly IAsyncDocumentSession _session;

        protected RavenUnitOfWorkBase(IDocumentStore store)
        {
            if (store == null)
                throw new ArgumentNullException(nameof(store));

            _session = store.OpenAsyncSession();
        }

        public override async Task CompleteAsync(CancellationToken cancellationToken)
        {
            await _session.SaveChangesAsync(cancellationToken);

            _session.Advanced.Clear();
        }

        public override void Dispose()
        {
            _session.Dispose();

            base.Dispose();
        }

        protected override bool IsDirty()
        {
            return false;
            return _session.Advanced.HasChanges;
        }
    }
}