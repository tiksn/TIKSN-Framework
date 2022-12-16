using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using NoDb;

namespace TIKSN.Data.NoDB
{
    public class NoDbRepository<TEntity, TIdentity> : IRepository<TEntity>, IQueryRepository<TEntity, TIdentity>,
        IStreamRepository<TEntity>
        where TEntity : class, IEntity<TIdentity>
        where TIdentity : IEquatable<TIdentity>
    {
        private readonly IBasicCommands<TEntity> _basicCommands;
        private readonly IBasicQueries<TEntity> _basicQueries;
        private readonly string _projectId;

        public NoDbRepository(IBasicCommands<TEntity> basicCommands, IBasicQueries<TEntity> basicQueries,
            IOptions<NoDbRepositoryOptions> genericOptions, IOptions<NoDbRepositoryOptions<TEntity>> specificOptions)
        {
            this._basicCommands = basicCommands;
            this._basicQueries = basicQueries;
            this._projectId = string.IsNullOrEmpty(genericOptions.Value.ProjectId)
                ? specificOptions.Value.ProjectId
                : genericOptions.Value.ProjectId;
        }

        public async Task<bool> ExistsAsync(TIdentity id, CancellationToken cancellationToken) =>
            await this._basicQueries.FetchAsync(this._projectId, id.ToString(), cancellationToken).ConfigureAwait(false) != null;

        public async Task<TEntity> GetAsync(TIdentity id, CancellationToken cancellationToken)
        {
            var result = await this.GetOrDefaultAsync(id, cancellationToken).ConfigureAwait(false);

            if (result == null)
            {
                throw new NullReferenceException("Result retrieved from database is null.");
            }

            return result;
        }

        public Task<TEntity> GetOrDefaultAsync(TIdentity id, CancellationToken cancellationToken) =>
            this._basicQueries.FetchAsync(this._projectId, id.ToString(), cancellationToken);

        public async Task<IEnumerable<TEntity>>
            ListAsync(IEnumerable<TIdentity> ids, CancellationToken cancellationToken) =>
            await BatchOperationHelper.BatchOperationAsync(ids, cancellationToken, this.GetAsync).ConfigureAwait(false);

        public Task AddAsync(TEntity entity, CancellationToken cancellationToken) =>
            this._basicCommands.CreateAsync(this._projectId, entity.ID.ToString(), entity, cancellationToken);

        public Task AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken) =>
            BatchOperationHelper.BatchOperationAsync(entities, cancellationToken, this.AddAsync);

        public Task RemoveAsync(TEntity entity, CancellationToken cancellationToken) =>
            this._basicCommands.DeleteAsync(this._projectId, entity.ID.ToString(), cancellationToken);

        public Task RemoveRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken) =>
            BatchOperationHelper.BatchOperationAsync(entities, cancellationToken, this.RemoveAsync);

        public Task UpdateAsync(TEntity entity, CancellationToken cancellationToken) =>
            this._basicCommands.UpdateAsync(this._projectId, entity.ID.ToString(), entity, cancellationToken);

        public Task UpdateRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken) =>
            BatchOperationHelper.BatchOperationAsync(entities, cancellationToken, this.UpdateAsync);

        public async IAsyncEnumerable<TEntity> StreamAllAsync(CancellationToken cancellationToken)
        {
            var entities = await this._basicQueries.GetAllAsync(this._projectId, cancellationToken).ConfigureAwait(false);
            foreach (var entity in entities)
            {
                yield return entity;
            }
        }

        protected Task<IEnumerable<TEntity>> GetAllAsync(CancellationToken cancellationToken) =>
            this._basicQueries.GetAllAsync(this._projectId, cancellationToken);
    }
}
