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
        protected readonly IMongoClientSessionProvider mongoClientSessionProvider;

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
            Task None() => this.collection.InsertOneAsync(entity, null, cancellationToken);

            Task Some(IClientSessionHandle clientSessionHandle) => this.collection.InsertOneAsync(clientSessionHandle, entity, null, cancellationToken);

            return this.mongoClientSessionProvider.GetClientSessionHandle().Match(Some, None);
        }

        [Obsolete]
        public Task AddOrUpdateAsync(TDocument entity, CancellationToken cancellationToken)
        {
            var updateOptions = new UpdateOptions { IsUpsert = true };

            Task None() => this.collection.ReplaceOneAsync(item => item.ID.Equals(entity.ID), entity, updateOptions,
                    cancellationToken);

            Task Some(IClientSessionHandle clientSessionHandle) => this.collection.ReplaceOneAsync(clientSessionHandle, item => item.ID.Equals(entity.ID), entity,
                    updateOptions, cancellationToken);

            return this.mongoClientSessionProvider.GetClientSessionHandle().Match(Some, None);
        }

        public Task AddRangeAsync(IEnumerable<TDocument> entities, CancellationToken cancellationToken)
        {
            Task None() => this.collection.InsertManyAsync(entities, cancellationToken: cancellationToken);

            Task Some(IClientSessionHandle clientSessionHandle) => this.collection.InsertManyAsync(clientSessionHandle, entities,
                    cancellationToken: cancellationToken);

            return this.mongoClientSessionProvider.GetClientSessionHandle().Match(Some, None);
        }

        public Task<bool> ExistsAsync(TIdentity id, CancellationToken cancellationToken)
        {
            Task<bool> None() => this.collection.Find(GetIdentityFilter(id)).AnyAsync(cancellationToken);

            Task<bool> Some(IClientSessionHandle clientSessionHandle) => this.collection.Find(clientSessionHandle, GetIdentityFilter(id)).AnyAsync(cancellationToken);

            return this.mongoClientSessionProvider.GetClientSessionHandle().Match(Some, None);
        }

        public Task<TDocument> GetAsync(TIdentity id, CancellationToken cancellationToken)
        {
            Task<TDocument> None() => this.collection.Find(GetIdentityFilter(id)).SingleAsync(cancellationToken);

            Task<TDocument> Some(IClientSessionHandle clientSessionHandle) => this.collection.Find(clientSessionHandle, GetIdentityFilter(id)).SingleAsync(cancellationToken);

            return this.mongoClientSessionProvider.GetClientSessionHandle().Match(Some, None);
        }

        public Task<TDocument> GetOrDefaultAsync(TIdentity id, CancellationToken cancellationToken)
        {
            Task<TDocument> None() => this.collection.Find(GetIdentityFilter(id)).SingleOrDefaultAsync(cancellationToken);

            Task<TDocument> Some(IClientSessionHandle clientSessionHandle) => this.collection.Find(clientSessionHandle, GetIdentityFilter(id))
                    .SingleOrDefaultAsync(cancellationToken);

            return this.mongoClientSessionProvider.GetClientSessionHandle().Match(Some, None);
        }

        public async Task<IEnumerable<TDocument>> ListAsync(IEnumerable<TIdentity> ids,
            CancellationToken cancellationToken)
        {
            async Task<IEnumerable<TDocument>> None() => await this.collection.Find(GetIdentitiesFilter(ids)).ToListAsync(cancellationToken).ConfigureAwait(false);

            async Task<IEnumerable<TDocument>> Some(IClientSessionHandle clientSessionHandle) => await this.collection.Find(clientSessionHandle, GetIdentitiesFilter(ids))
                    .ToListAsync(cancellationToken).ConfigureAwait(false);

            return await this.mongoClientSessionProvider.GetClientSessionHandle().Match(Some, None).ConfigureAwait(false);
        }

        public Task RemoveAsync(TDocument entity, CancellationToken cancellationToken)
        {
            Task None() => this.collection.DeleteOneAsync(item => item.ID.Equals(entity.ID), cancellationToken);

            Task Some(IClientSessionHandle clientSessionHandle) => this.collection.DeleteOneAsync(clientSessionHandle, item => item.ID.Equals(entity.ID),
                    cancellationToken: cancellationToken);

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

            Task None() => this.collection.DeleteManyAsync(filter, cancellationToken);

            Task Some(IClientSessionHandle clientSessionHandle) => this.collection.DeleteManyAsync(clientSessionHandle, filter, null, cancellationToken);

            return this.mongoClientSessionProvider.GetClientSessionHandle().Match(Some, None);
        }

        public async IAsyncEnumerable<TDocument> StreamAllAsync(CancellationToken cancellationToken)
        {
            Task<IAsyncCursor<TDocument>> None() => this.collection.Find(FilterDefinition<TDocument>.Empty).ToCursorAsync(cancellationToken);

            Task<IAsyncCursor<TDocument>> Some(IClientSessionHandle clientSessionHandle) => this.collection.Find(clientSessionHandle, FilterDefinition<TDocument>.Empty)
                    .ToCursorAsync(cancellationToken);

            var cursor = await this.mongoClientSessionProvider.GetClientSessionHandle().Match(Some, None).ConfigureAwait(false);

            while (await cursor.MoveNextAsync(cancellationToken).ConfigureAwait(false))
            {
                foreach (var entity in cursor.Current)
                {
                    yield return entity;
                }
            }
        }

        public Task UpdateAsync(TDocument entity, CancellationToken cancellationToken)
        {
            Task None() => this.collection.ReplaceOneAsync(item => item.ID.Equals(entity.ID), entity,
                    cancellationToken: cancellationToken);

            Task Some(IClientSessionHandle clientSessionHandle) => this.collection.ReplaceOneAsync(clientSessionHandle, item => item.ID.Equals(entity.ID), entity,
                    cancellationToken: cancellationToken);

            return this.mongoClientSessionProvider.GetClientSessionHandle().Match(Some, None);
        }

        public Task UpdateRangeAsync(IEnumerable<TDocument> entities, CancellationToken cancellationToken) =>
            BatchOperationHelper.BatchOperationAsync(entities, cancellationToken, this.UpdateAsync);

        protected static FilterDefinition<TDocument> GetIdentitiesFilter(IEnumerable<TIdentity> ids) =>
            Builders<TDocument>.Filter.In(item => item.ID, ids);

        protected static FilterDefinition<TDocument> GetIdentityFilter(TIdentity id) =>
            Builders<TDocument>.Filter.Eq(item => item.ID, id);

        protected Task<TDocument> SingleOrDefaultAsync(
            FilterDefinition<TDocument> filter,
            CancellationToken cancellationToken)
        {
            Task<TDocument> None() => this.collection.Find(filter)
                .SingleOrDefaultAsync(cancellationToken);

            Task<TDocument> Some(IClientSessionHandle clientSessionHandle) => this.collection.Find(clientSessionHandle, filter)
                .SingleOrDefaultAsync(cancellationToken);

            return this.mongoClientSessionProvider.GetClientSessionHandle().Match(Some, None);
        }

        protected async Task<IReadOnlyCollection<TDocument>> SearchAsync(
            FilterDefinition<TDocument> filter,
            CancellationToken cancellationToken)
        {
            Task<List<TDocument>> None() => this.collection.Find(filter)
                .ToListAsync(cancellationToken);

            Task<List<TDocument>> Some(IClientSessionHandle clientSessionHandle) => this.collection.Find(clientSessionHandle, filter)
                .ToListAsync(cancellationToken);

            return await this.mongoClientSessionProvider.GetClientSessionHandle().Match(Some, None);
        }
    }
}
