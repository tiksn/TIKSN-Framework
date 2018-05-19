using System;
using System.Threading;
using System.Threading.Tasks;
using Realms;

namespace TIKSN.Data.Realm
{
    public abstract class RealmUnitOfWorkBase : UnitOfWorkBase, IRealmUnitOfWork
    {
        private readonly Transaction _transaction;

        protected RealmUnitOfWorkBase(Realms.Realm realm)
        {
            Realm = realm ?? throw new ArgumentNullException(nameof(realm));
            _transaction = realm.BeginWrite();
        }

        public Realms.Realm Realm { get; }

        protected override bool IsDirty()
        {
            return Realm.IsInTransaction;
        }

        public override Task CompleteAsync(CancellationToken cancellationToken)
        {
            _transaction.Commit();

            return Task.CompletedTask;
        }
    }
}
