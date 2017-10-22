using System;
using System.Threading;
using System.Threading.Tasks;

namespace TIKSN.Data
{
    public interface IQueryRepository<TEntity, TIdentity>
        where TEntity : IEntity<TIdentity>
        where TIdentity : IEquatable<TIdentity>
    {
        Task<TEntity> GetAsync(TIdentity id, CancellationToken cancellationToken = default(CancellationToken));
    }
}