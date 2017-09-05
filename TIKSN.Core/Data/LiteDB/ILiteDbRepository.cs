using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace TIKSN.Data.LiteDB
{
	public interface ILiteDbRepository<TDocument, TIdentity> : IRepository<TDocument>, IQueryRepository<TDocument, TIdentity> where TDocument : IEntity<TIdentity> where TIdentity : IEquatable<TIdentity>
	{
		Task AddOrUpdateAsync(TDocument entity, CancellationToken cancellationToken = default);

		Task AddOrUpdateRangeAsync(IEnumerable<TDocument> entities, CancellationToken cancellationToken = default);
	}
}
