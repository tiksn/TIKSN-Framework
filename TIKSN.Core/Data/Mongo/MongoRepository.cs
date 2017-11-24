using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace TIKSN.Data.Mongo
{
    public class MongoRepository<TDocument, TIdentity> : IMongoRepository<TDocument, TIdentity> where TDocument : IEntity<TIdentity> where TIdentity : IEquatable<TIdentity>
    {
        protected readonly IMongoCollection<TDocument> collection;

        protected MongoRepository(IMongoDatabaseProvider mongoDatabaseProvider, string collectionName)
        {
            var database = mongoDatabaseProvider.GetDatabase();
            collection = database.GetCollection<TDocument>(collectionName);
        }

        public Task AddAsync(TDocument entity, CancellationToken cancellationToken) => collection.InsertOneAsync(entity, null, cancellationToken);

        public Task AddOrUpdateAsync(TDocument entity, CancellationToken cancellationToken) => collection.ReplaceOneAsync(item => item.ID.Equals(entity.ID), entity, new UpdateOptions { IsUpsert = true }, cancellationToken);

        public Task AddRangeAsync(IEnumerable<TDocument> entities, CancellationToken cancellationToken) => collection.InsertManyAsync(entities, cancellationToken: cancellationToken);

        public Task<TDocument> GetAsync(TIdentity id, CancellationToken cancellationToken) => collection.Find(GetIdentityFilter(id)).SingleOrDefaultAsync(cancellationToken);

        public Task RemoveAsync(TDocument entity, CancellationToken cancellationToken) => collection.DeleteOneAsync(item => item.ID.Equals(entity.ID), cancellationToken: cancellationToken);

        public Task RemoveRangeAsync(IEnumerable<TDocument> entities, CancellationToken cancellationToken)
        {
            if (entities == null)
                throw new ArgumentNullException(nameof(entities));

            var filters = entities.Select(item => GetIdentityFilter(item.ID)).ToArray();

            if (filters.Length == 0)
                return Task.FromResult<object>(null);

            var filter = Builders<TDocument>.Filter.Or(filters);

            return collection.DeleteManyAsync(filter, cancellationToken);
        }

        public Task UpdateAsync(TDocument entity, CancellationToken cancellationToken) => collection.ReplaceOneAsync(item => item.ID.Equals(entity.ID), entity, cancellationToken: cancellationToken);

        public Task UpdateRangeAsync(IEnumerable<TDocument> entities, CancellationToken cancellationToken) => BatchOperationHelper.BatchOperationAsync(entities, cancellationToken, UpdateAsync);

        protected static FilterDefinition<TDocument> GetIdentityFilter(TIdentity id)
        {
            return Builders<TDocument>.Filter.Eq(item => item.ID, id);
        }
    }
}