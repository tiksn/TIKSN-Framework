﻿using System;
using System.Threading;
using System.Threading.Tasks;
using TIKSN.Data;

namespace TIKSN.Web.Rest
{
	public interface IRestRepository<TEntity, TIdentity> where TEntity : IEntity<TIdentity> where TIdentity : IEquatable<TIdentity>
	{
		Task AddAsync(TEntity entity, CancellationToken cancellationToken = default(CancellationToken));

		Task<TEntity> GetAsync(TIdentity id, CancellationToken cancellationToken = default(CancellationToken));

		Task RemoveAsync(TEntity entity, CancellationToken cancellationToken = default(CancellationToken));

		Task UpdateAsync(TEntity entity, CancellationToken cancellationToken = default(CancellationToken));
	}
}
