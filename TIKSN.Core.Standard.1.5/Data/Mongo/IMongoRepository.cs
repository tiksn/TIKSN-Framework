using System;
using System.Threading;
using System.Threading.Tasks;

namespace TIKSN.Data.Mongo
{
	public interface IMongoRepository<TDocument, TField> : IRepository<TDocument> where TDocument : IEntity<TField> where TField : IEquatable<TField>
	{
		Task AddOrUpdateAsync(TDocument entity, CancellationToken cancellationToken = default(CancellationToken));

		Task<TDocument> GetAsync(TField id, CancellationToken cancellationToken = default(CancellationToken));
	}
}