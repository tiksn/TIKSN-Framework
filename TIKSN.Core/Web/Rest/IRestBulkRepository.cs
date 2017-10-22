using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TIKSN.Data;

namespace TIKSN.Web.Rest
{
    public interface IRestBulkRepository<TEntity, TIdentity> where TEntity : IEntity<TIdentity> where TIdentity : IEquatable<TIdentity>
    {
        Task AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default(CancellationToken));

        Task<TEntity> GetAsync(TIdentity id, CancellationToken cancellationToken = default(CancellationToken));

        Task RemoveAsync(TEntity entity, CancellationToken cancellationToken = default(CancellationToken));

        Task UpdateRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default(CancellationToken));
    }
}