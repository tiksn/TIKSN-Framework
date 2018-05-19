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
            throw new NotImplementedException();
        }

        public Task AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<TEntity> GetAsync(TIdentity id, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<TEntity> GetOrDefaultAsync(TIdentity id, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task RemoveAsync(TEntity entity, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task RemoveRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task UpdateAsync(TEntity entity, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task UpdateRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}