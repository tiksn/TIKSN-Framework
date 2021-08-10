using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MongoDB.Driver;

namespace TIKSN.Data.Mongo
{
    public class MongoRepository<TDocument, TIdentity> : IMongoRepository<TDocument, TIdentity>
        where TDocument : IEntity<TIdentity> where TIdentity : IEquatable<TIdentity>
    {
        protected readonly IMongoCollection<TDocument> collection;
        private readonly IMongoClientSessionProvider mongoClientSessionProvider;

        protected MongoRepository(IMongoClientSessionProvider mongoClientSessionProvider,
            IMongoDatabaseProvider mongoDatabaseProvider, string collectionName)
        {
            this.mongoClientSessionProvider = mongoClientSessionProvider ??
                                              throw new ArgumentNullException(nameof(mongoClientSessionProvider));
            var database = mongoDatabaseProvider.GetDatabase();
            this.collection = database.GetCollection<TDocument>(collectionName);
        }

        public Task AddAsync(TDocument entity, CancellationToken cancellationToken)
        {
            Task None()
            {
                return this.collection.InsertOneAsync(entity, null, cancellationToken);
            }

            Task Some(IClientSessionHandle clientSessionHandle)
            {
                return this.collection.InsertOneAsync(clientSessionHandle, entity, null, cancellationToken);
            }

            return this.mongoClientSessionProvider.GetClientSessionHandle().Match(Some, None);
        }

        public Task AddOrUpdateAsync(TDocument entity, CancellationToken cancellationToken)
        {
            var updateOptions = new UpdateOptions { IsUpsert = true };

            Task None()
            {
                return this.collection.ReplaceOneAsync(item => item.ID.Equals(entity.ID), entity, updateOptions,
                    cancellationToken);
            }

            Task Some(IClientSessionHandle clientSessionHandle)
            {
                return this.collection.ReplaceOneAsync(clientSessionHandle, item => item.ID.Equals(entity.ID), entity,
                    updateOptions, cancellationToken);
            }

            return this.mongoClientSessionProvider.GetClientSessionHandle().Match(Some, None);
        }

        public Task AddRangeAsync(IEnumerable<TDocument> entities, CancellationToken cancellationToken)
        {
            Task None()
            {
                return this.collection.InsertManyAsync(entities, cancellationToken: cancellationToken);
            }

            Task Some(IClientSessionHandle clientSessionHandle)
            {
                return this.collection.InsertManyAsync(clientSessionHandle, entities,
                    cancellationToken: cancellationToken);
            }

            return this.mongoClientSessionProvider.GetClientSessionHandle().Match(Some, None);
        }

        public Task<bool> ExistsAsync(TIdentity id, CancellationToken cancellationToken)
        {
            Task<bool> None()
            {
                return this.collection.Find(GetIdentityFilter(id)).AnyAsync(cancellationToken);
            }

            Task<bool> Some(IClientSessionHandle clientSessionHandle)
            {
                return this.collection.Find(clientSessionHandle, GetIdentityFilter(id)).AnyAsync(cancellationToken);
            }

            return this.mongoClientSessionProvider.GetClientSessionHandle().Match(Some, None);
        }

        public Task<TDocument> GetAsync(TIdentity id, CancellationToken cancellationToken)
        {
            Task<TDocument> None()
            {
                return this.collection.Find(GetIdentityFilter(id)).SingleAsync(cancellationToken);
            }

            Task<TDocument> Some(IClientSessionHandle clientSessionHandle)
            {
                return this.collection.Find(clientSessionHandle, GetIdentityFilter(id)).SingleAsync(cancellationToken);
            }

            return this.mongoClientSessionProvider.GetClientSessionHandle().Match(Some, None);
        }

        public Task<TDocument> GetOrDefaultAsync(TIdentity id, CancellationToken cancellationToken)
        {
            Task<TDocument> None()
            {
                return this.collection.Find(GetIdentityFilter(id)).SingleOrDefaultAsync(cancellationToken);
            }

            Task<TDocument> Some(IClientSessionHandle clientSessionHandle)
            {
                return this.collection.Find(clientSessionHandle, GetIdentityFilter(id))
                    .SingleOrDefaultAsync(cancellationToken);
            }

            return this.mongoClientSessionProvider.GetClientSessionHandle().Match(Some, None);
        }

        public async Task<IEnumerable<TDocument>> ListAsync(IEnumerable<TIdentity> ids,
            CancellationToken cancellationToken)
        {
            async Task<IEnumerable<TDocument>> None()
            {
                return await this.collection.Find(GetIdentitiesFilter(ids)).ToListAsync(cancellationToken);
            }

            async Task<IEnumerable<TDocument>> Some(IClientSessionHandle clientSessionHandle)
            {
                return await this.collection.Find(clientSessionHandle, GetIdentitiesFilter(ids))
                    .ToListAsync(cancellationToken);
            }

            return await this.mongoClientSessionProvider.GetClientSessionHandle().Match(Some, None);
        }

        public Task RemoveAsync(TDocument entity, CancellationToken cancellationToken)
        {
            Task None()
            {
                return this.collection.DeleteOneAsync(item => item.ID.Equals(entity.ID), cancellationToken);
            }

            Task Some(IClientSessionHandle clientSessionHandle)
            {
                return this.collection.DeleteOneAsync(clientSessionHandle, item => item.ID.Equals(entity.ID),
                    cancellationToken: cancellationToken);
            }

            return this.mongoClientSessionProvider.GetClientSessionHandle().Match(Some, None);
        }

        public Task RemoveRangeAsync(IEnumerable<TDocument> entities, CancellationToken cancellationToken)
        {
            if (entities == null)
            {
                throw new ArgumentNullException(nameof(entities));
            }

            var filters = entities.Select(item => GetIdentityFilter(item.ID)).ToArray();

            if (filters.Length == 0)
            {
                return Task.FromResult<object>(null);
            }

            var filter = Builders<TDocument>.Filter.Or(filters);

            Task None()
            {
                return this.collection.DeleteManyAsync(filter, cancellationToken);
            }

            Task Some(IClientSessionHandle clientSessionHandle)
            {
                return this.collection.DeleteManyAsync(clientSessionHandle, filter, null, cancellationToken);
            }

            return this.mongoClientSessionProvider.GetClientSessionHandle().Match(Some, None);
        }

        public async IAsyncEnumerable<TDocument> StreamAllAsync(CancellationToken cancellationToken)
        {
            Task<IAsyncCursor<TDocument>> None()
            {
                return this.collection.Find(FilterDefinition<TDocument>.Empty).ToCursorAsync(cancellationToken);
            }

            Task<IAsyncCursor<TDocument>> Some(IClientSessionHandle clientSessionHandle)
            {
                return this.collection.Find(clientSessionHandle, FilterDefinition<TDocument>.Empty)
                    .ToCursorAsync(cancellationToken);
            }

            var cursor = await this.mongoClientSessionProvider.GetClientSessionHandle().Match(Some, None);

            while (await cursor.MoveNextAsync(cancellationToken))
            {
                foreach (var entity in cursor.Current)
                {
                    yield return entity;
                }
            }
        }

        public Task UpdateAsync(TDocument entity, CancellationToken cancellationToken)
        {
            Task None()
            {
                return this.collection.ReplaceOneAsync(item => item.ID.Equals(entity.ID), entity,
                    cancellationToken: cancellationToken);
            }

            Task Some(IClientSessionHandle clientSessionHandle)
            {
                return this.collection.ReplaceOneAsync(clientSessionHandle, item => item.ID.Equals(entity.ID), entity,
                    cancellationToken: cancellationToken);
            }

            return this.mongoClientSessionProvider.GetClientSessionHandle().Match(Some, None);
        }

        public Task UpdateRangeAsync(IEnumerable<TDocument> entities, CancellationToken cancellationToken) =>
            BatchOperationHelper.BatchOperationAsync(entities, cancellationToken, this.UpdateAsync);

        protected static FilterDefinition<TDocument> GetIdentitiesFilter(IEnumerable<TIdentity> ids) =>
            Builders<TDocument>.Filter.In(item => item.ID, ids);

        protected static FilterDefinition<TDocument> GetIdentityFilter(TIdentity id) =>
            Builders<TDocument>.Filter.Eq(item => item.ID, id);
    }
}
