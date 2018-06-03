﻿using Raven.Client.Documents.Session;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace TIKSN.Data.RavenDB
{
    public class RavenRepository<TEntity, TIdentity> : IRepository<TEntity>, IQueryRepository<TEntity, TIdentity>
        where TEntity : IEntity<TIdentity>
        where TIdentity : IEquatable<TIdentity>
    {
        private readonly IAsyncDocumentSession _session;

        public RavenRepository(IAsyncDocumentSession session)
        {
            _session = session ?? throw new ArgumentNullException(nameof(session));
        }

        public Task AddAsync(TEntity entity, CancellationToken cancellationToken)
        {
            return _session.StoreAsync(entity, entity.ID.ToString(), cancellationToken);
        }

        public Task AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken)
        {
            return BatchOperationHelper.BatchOperationAsync(entities, cancellationToken, AddAsync);
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
            _session.Delete(entity.ID.ToString());

            return Task.CompletedTask;
        }

        public Task RemoveRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken)
        {
            return BatchOperationHelper.BatchOperationAsync(entities, cancellationToken, RemoveAsync);
        }

        public Task UpdateAsync(TEntity entity, CancellationToken cancellationToken)
        {
            return _session.StoreAsync(entity, entity.ID.ToString(), cancellationToken);
        }

        public Task UpdateRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken)
        {
            return BatchOperationHelper.BatchOperationAsync(entities, cancellationToken, UpdateAsync);
        }
    }
}
