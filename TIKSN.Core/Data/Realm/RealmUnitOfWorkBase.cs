using Realms;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace TIKSN.Data.Realm
{
    public abstract class RealmUnitOfWorkBase : UnitOfWorkBase
    {
        private readonly Realms.Realm _realm;
        private readonly Transaction _transaction;
        private bool _completed;

        protected RealmUnitOfWorkBase(Realms.Realm realm)
        {
            _realm = realm ?? throw new ArgumentNullException(nameof(realm));

            _transaction = realm.BeginWrite();

            _completed = false;
        }

        public override Task CompleteAsync(CancellationToken cancellationToken)
        {
            _transaction.Commit();

            _completed = true;

            return Task.CompletedTask;
        }

        protected override bool IsDirty()
        {
            return !_completed;
        }
    }
}