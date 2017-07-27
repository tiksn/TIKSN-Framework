using Microsoft.Extensions.Options;
using NoDb;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace TIKSN.Data.NoDB
{
	public class NoDbRepository<TEntity, TIdentity> : IRepository<TEntity>
		where TEntity : class, IEntity<TIdentity>
		where TIdentity : IEquatable<TIdentity>
	{
		private readonly IBasicCommands<TEntity> _basicCommands;
		private readonly IBasicQueries<TEntity> _basicQueries;
		private readonly string _projectId;

		public NoDbRepository(IBasicCommands<TEntity> basicCommands, IBasicQueries<TEntity> basicQueries, IOptions<NoDbRepositoryOptions> genericOptions, IOptions<NoDbRepositoryOptions<TEntity>> specificOptions)
		{
			_basicCommands = basicCommands;
			_basicQueries = basicQueries;
			_projectId = string.IsNullOrEmpty(genericOptions.Value.ProjectId) ? specificOptions.Value.ProjectId : genericOptions.Value.ProjectId;
		}

		public Task AddAsync(TEntity entity, CancellationToken cancellationToken)
		{
			return _basicCommands.CreateAsync(_projectId, entity.ID.ToString(), entity, cancellationToken);
		}

		public Task AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken)
		{
			return BatchOperationHelper.BatchOperationAsync(entities, cancellationToken, (e, c) => AddAsync(e, c));
		}

		public Task RemoveAsync(TEntity entity, CancellationToken cancellationToken)
		{
			return _basicCommands.DeleteAsync(_projectId, entity.ID.ToString(), cancellationToken);
		}

		public Task RemoveRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken)
		{
			return BatchOperationHelper.BatchOperationAsync(entities, cancellationToken, (e, c) => RemoveAsync(e, c));
		}

		public Task UpdateAsync(TEntity entity, CancellationToken cancellationToken)
		{
			return _basicCommands.UpdateAsync(_projectId, entity.ID.ToString(), entity, cancellationToken);
		}

		public Task UpdateRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken)
		{
			return BatchOperationHelper.BatchOperationAsync(entities, cancellationToken, (e, c) => UpdateAsync(e, c));
		}

		protected Task<TEntity> GetAsync(TIdentity id, CancellationToken cancellationToken)
		{
			return _basicQueries.FetchAsync(_projectId, id.ToString(), cancellationToken);
		}

		protected Task<IEnumerable<TEntity>> GetAllAsync(CancellationToken cancellationToken)
		{
			return _basicQueries.GetAllAsync(_projectId, cancellationToken);
		}
	}
}
