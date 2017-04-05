using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MongoDB.Driver;

namespace TIKSN.Data.Mongo
{
	public class MongoRepository<T> : IMongoRepository<T>
	{
		private readonly IMongoCollection<T> collection;

		public MongoRepository(IMongoDatabaseProvider mongoDatabaseProvider, string collectionName)
		{
			var database = mongoDatabaseProvider.GetDatabase();
			collection = database.GetCollection<T>(collectionName);
		}

		public Task AddAsync(T entity, CancellationToken cancellationToken) => collection.InsertOneAsync(entity, cancellationToken);

		public Task AddRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken) => collection.InsertManyAsync(entities, cancellationToken: cancellationToken);

		public Task RemoveAsync(T entity, CancellationToken cancellationToken)
		{
			throw new NotImplementedException();
		}

		public Task RemoveRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken)
		{
			throw new NotImplementedException();
		}

		public Task UpdateAsync(T entity, CancellationToken cancellationToken)
		{
			throw new NotImplementedException();
		}

		public Task UpdateRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken)
		{
			throw new NotImplementedException();
		}
	}
}