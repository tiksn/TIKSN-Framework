using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LiteDB;

namespace TIKSN.Data.LiteDB
{
    public class LiteDbRepository<TDocument, TIdentity> : ILiteDbRepository<TDocument, TIdentity>
        where TDocument : IEntity<TIdentity> where TIdentity : IEquatable<TIdentity>
    {
        protected readonly ILiteCollection<TDocument> collection;
        protected readonly Func<TIdentity, BsonValue> convertToBsonValue;

        protected LiteDbRepository(ILiteDbDatabaseProvider databaseProvider, string collectionName,
            Func<TIdentity, BsonValue> convertToBsonValue)
        {
            var database = databaseProvider.GetDatabase();
            this.collection = database.GetCollection<TDocument>(collectionName);
            this.convertToBsonValue = convertToBsonValue ?? throw new ArgumentNullException(nameof(convertToBsonValue));
        }

        public Task AddAsync(TDocument entity, CancellationToken cancellationToken)
        {
            _ = this.collection.Insert(entity);
            return Task.CompletedTask;
        }

        public Task AddOrUpdateAsync(TDocument entity, CancellationToken cancellationToken)
        {
            _ = this.collection.Upsert(entity);
            return Task.CompletedTask;
        }

        public Task AddOrUpdateRangeAsync(IEnumerable<TDocument> entities, CancellationToken cancellationToken)
        {
            _ = this.collection.Upsert(entities);
            return Task.CompletedTask;
        }

        public Task AddRangeAsync(IEnumerable<TDocument> entities, CancellationToken cancellationToken)
        {
            _ = this.collection.Insert(entities);
            return Task.CompletedTask;
        }

        public Task<bool> ExistsAsync(TIdentity id, CancellationToken cancellationToken) =>
            Task.FromResult(this.collection.Exists(item => item.ID.Equals(id)));

        public async Task<TDocument> GetAsync(TIdentity id, CancellationToken cancellationToken)
        {
            var result = await this.GetOrDefaultAsync(id, cancellationToken).ConfigureAwait(false);

            if (result == null)
            {
                throw new NullReferenceException("Result retrieved from database is null.");
            }

            return result;
        }

        public Task<TDocument> GetOrDefaultAsync(TIdentity id, CancellationToken cancellationToken) =>
            Task.FromResult(this.collection.FindOne(item => item.ID.Equals(id)));

        public Task<IEnumerable<TDocument>>
            ListAsync(IEnumerable<TIdentity> ids, CancellationToken cancellationToken) =>
            Task.FromResult<IEnumerable<TDocument>>(this.collection.Find(item => ids.Contains(item.ID)).ToArray());

        public Task RemoveAsync(TDocument entity, CancellationToken cancellationToken) =>
            Task.FromResult(this.collection.Delete(this.convertToBsonValue(entity.ID)));

        public Task RemoveRangeAsync(IEnumerable<TDocument> entities, CancellationToken cancellationToken)
        {
            if (entities == null)
            {
                throw new ArgumentNullException(nameof(entities));
            }

            var ids = entities.Select(item => item.ID).ToArray();

            _ = this.collection.DeleteMany(item => ids.Contains(item.ID));
            return Task.CompletedTask;
        }

        public IAsyncEnumerable<TDocument> StreamAllAsync(CancellationToken cancellationToken) =>
            this.collection.FindAll().ToAsyncEnumerable();

        public Task UpdateAsync(TDocument entity, CancellationToken cancellationToken)
        {
            _ = this.collection.Update(entity);
            return Task.CompletedTask;
        }

        public Task UpdateRangeAsync(IEnumerable<TDocument> entities, CancellationToken cancellationToken)
        {
            _ = this.collection.Update(entities);
            return Task.CompletedTask;
        }
    }
}
