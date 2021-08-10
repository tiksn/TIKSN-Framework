using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Raven.Client.Documents;
using Raven.Client.Documents.Linq;
using Raven.Client.Documents.Session;

namespace TIKSN.Data.RavenDB
{
    public class RavenRepository<TEntity, TIdentity> : IRepository<TEntity>, IQueryRepository<TEntity, TIdentity>,
        IStreamRepository<TEntity>
        where TEntity : IEntity<TIdentity>
        where TIdentity : IEquatable<TIdentity>
    {
        private readonly IAsyncDocumentSession _session;

        public RavenRepository(IAsyncDocumentSession session) =>
            this._session = session ?? throw new ArgumentNullException(nameof(session));

        public Task<bool> ExistsAsync(TIdentity id, CancellationToken cancellationToken) =>
            this._session.Advanced.ExistsAsync(id.ToString(), cancellationToken);

        public async Task<TEntity> GetAsync(TIdentity id, CancellationToken cancellationToken)
        {
            var entity = await this.GetOrDefaultAsync(id, cancellationToken).ConfigureAwait(false);

            if (entity is null)
            {
                throw new NullReferenceException($"Entity with ID '{id}' is not found.");
            }

            return entity;
        }

        public Task<TEntity> GetOrDefaultAsync(TIdentity id, CancellationToken cancellationToken) =>
            this._session.LoadAsync<TEntity>(id.ToString(), cancellationToken);

        public async Task<IEnumerable<TEntity>> ListAsync(IEnumerable<TIdentity> ids,
            CancellationToken cancellationToken) =>
            await this._session.Query<TEntity>()
                .Where(entity => ids.Contains(entity.ID))
                .ToListAsync(cancellationToken).ConfigureAwait(false);

        public Task AddAsync(TEntity entity, CancellationToken cancellationToken) =>
            this._session.StoreAsync(entity, entity.ID.ToString(), cancellationToken);

        public Task AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken) =>
            BatchOperationHelper.BatchOperationAsync(entities, cancellationToken, this.AddAsync);

        public Task RemoveAsync(TEntity entity, CancellationToken cancellationToken)
        {
            this._session.Delete(entity.ID.ToString());

            return Task.CompletedTask;
        }

        public Task RemoveRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken) =>
            BatchOperationHelper.BatchOperationAsync(entities, cancellationToken, this.RemoveAsync);

        public Task UpdateAsync(TEntity entity, CancellationToken cancellationToken) =>
            this._session.StoreAsync(entity, entity.ID.ToString(), cancellationToken);

        public Task UpdateRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken) =>
            BatchOperationHelper.BatchOperationAsync(entities, cancellationToken, this.UpdateAsync);

        public IAsyncEnumerable<TEntity> StreamAllAsync(CancellationToken cancellationToken) =>
            this._session.Query<TEntity>()
                .ToAsyncEnumerable();
    }
}
