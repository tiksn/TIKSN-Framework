using Realms;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace TIKSN.Data.Realm
{
    public class RealmRepository<TEntity, TIdentity> : IRepository<TEntity>, IQueryRepository<TEntity, TIdentity>
        where TIdentity : IEquatable<TIdentity>
        where TEntity : RealmObject, IEntity<TIdentity>
    {
        private readonly IRealmUnitOfWork _unitOfWork;

        public RealmRepository(IRealmUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        public Task AddAsync(TEntity entity, CancellationToken cancellationToken)
        {
            _unitOfWork.Realm.Add(entity);

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
                return Task.FromResult(_unitOfWork.Realm.Find<TEntity>(iid));

            if (id is string sid)
                return Task.FromResult(_unitOfWork.Realm.Find<TEntity>(sid));

            return Task.FromException<TEntity>(new InvalidCastException("ID must be either long or string."));
        }

        public Task RemoveAsync(TEntity entity, CancellationToken cancellationToken)
        {
            _unitOfWork.Realm.Remove(entity);

            return Task.CompletedTask;
        }

        public Task RemoveRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken)
        {
            return BatchOperationHelper.BatchOperationAsync(entities, cancellationToken, RemoveAsync);
        }

        public Task UpdateAsync(TEntity entity, CancellationToken cancellationToken)
        {
            _unitOfWork.Realm.Add(entity, true);

            return Task.CompletedTask;
        }

        public Task UpdateRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken)
        {
            return BatchOperationHelper.BatchOperationAsync(entities, cancellationToken, UpdateAsync);
        }
    }
}