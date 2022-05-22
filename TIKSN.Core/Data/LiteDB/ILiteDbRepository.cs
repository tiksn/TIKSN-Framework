using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace TIKSN.Data.LiteDB
{
    public interface ILiteDbRepository<TDocument, TIdentity> : IRepository<TDocument>,
        IQueryRepository<TDocument, TIdentity>,
        IStreamRepository<TDocument> where TDocument : IEntity<TIdentity> where TIdentity : IEquatable<TIdentity>
    {
        Task AddOrUpdateAsync(TDocument entity, CancellationToken cancellationToken);

        Task AddOrUpdateRangeAsync(IEnumerable<TDocument> entities, CancellationToken cancellationToken);
    }
}
