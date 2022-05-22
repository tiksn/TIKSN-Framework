using System;
using System.Threading;
using System.Threading.Tasks;

namespace TIKSN.Data.Mongo
{
    public interface IMongoRepository<TDocument, TIdentity> : IRepository<TDocument>,
        IQueryRepository<TDocument, TIdentity>,
        IStreamRepository<TDocument> where TDocument : IEntity<TIdentity> where TIdentity : IEquatable<TIdentity>
    {
        Task AddOrUpdateAsync(TDocument entity, CancellationToken cancellationToken);
    }
}
