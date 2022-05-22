using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace TIKSN.Data
{
    public interface IQueryRepository<TEntity, TIdentity>
        where TEntity : IEntity<TIdentity>
        where TIdentity : IEquatable<TIdentity>
    {
        Task<TEntity> GetAsync(TIdentity id, CancellationToken cancellationToken);

        Task<TEntity> GetOrDefaultAsync(TIdentity id, CancellationToken cancellationToken);

        Task<bool> ExistsAsync(TIdentity id, CancellationToken cancellationToken);

        Task<IEnumerable<TEntity>> ListAsync(IEnumerable<TIdentity> ids, CancellationToken cancellationToken);
    }
}
