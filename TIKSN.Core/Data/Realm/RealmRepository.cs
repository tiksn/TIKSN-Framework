using Realms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace TIKSN.Data.Realm
{
    public class RealmRepository<TEntity, TIdentity> : IRepository<TEntity>, IQueryRepository<TEntity, TIdentity>
        where TIdentity : IEquatable<TIdentity>
        where TEntity : RealmObject, IEntity<TIdentity>
    {
        private readonly Realms.Realm _realm;

        public RealmRepository(Realms.Realm realm)
        {
            _realm = realm ?? throw new ArgumentNullException(nameof(realm));
        }

        protected IQueryable<TEntity> Entities => _realm.All<TEntity>();

        public Task AddAsync(TEntity entity, CancellationToken cancellationToken)
        {
            _realm.Add(entity);

            return Task.CompletedTask;
        }

        public Task AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken)
        {
            return BatchOperationHelper.BatchOperationAsync(entities, cancellationToken, AddAsync);
        }

        public async Task<TEntity> GetAsync(TIdentity id, CancellationToken cancellationToken)
        {
            var entity = await GetOrDefaultAsync(id, cancellationToken);

            if (entity == null)
                throw new NullReferenceException($"No element were found with id '{id}'");

            return entity;
        }

        public Task<TEntity> GetOrDefaultAsync(TIdentity id, CancellationToken cancellationToken)
        {
            if (id is long iid)
                return Task.FromResult(_realm.Find<TEntity>(iid));

            if (id is string sid)
                return Task.FromResult(_realm.Find<TEntity>(sid));

            return Task.FromException<TEntity>(new InvalidCastException("ID must be either long or string."));
        }

        public Task RemoveAsync(TEntity entity, CancellationToken cancellationToken)
        {
            _realm.Remove(entity);

            return Task.CompletedTask;
        }

        public Task RemoveRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken)
        {
            return BatchOperationHelper.BatchOperationAsync(entities, cancellationToken, RemoveAsync);
        }

        public Task UpdateAsync(TEntity entity, CancellationToken cancellationToken)
        {
            _realm.Add(entity, true);

            return Task.CompletedTask;
        }

        public Task UpdateRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken)
        {
            return BatchOperationHelper.BatchOperationAsync(entities, cancellationToken, UpdateAsync);
        }
    }
}