using LiteDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace TIKSN.Data.LiteDB
{
    public class LiteDbRepository<TDocument, TIdentity> : ILiteDbRepository<TDocument, TIdentity> where TDocument : IEntity<TIdentity> where TIdentity : IEquatable<TIdentity>
    {
        protected readonly LiteCollection<TDocument> collection;

        protected LiteDbRepository(ILiteDbDatabaseProvider databaseProvider, string collectionName)
        {
            var database = databaseProvider.GetDatabase();
            collection = database.GetCollection<TDocument>(collectionName);
        }

        public Task AddAsync(TDocument entity, CancellationToken cancellationToken) => Task.Run(() => collection.Insert(entity), cancellationToken);

        public Task AddOrUpdateAsync(TDocument entity, CancellationToken cancellationToken) => Task.Run(() => collection.Upsert(entity), cancellationToken);

        public Task AddOrUpdateRangeAsync(IEnumerable<TDocument> entities, CancellationToken cancellationToken) => Task.Run(() => collection.Upsert(entities), cancellationToken);

        public Task AddRangeAsync(IEnumerable<TDocument> entities, CancellationToken cancellationToken) => Task.Run(() => collection.Insert(entities), cancellationToken);

        public Task<TDocument> GetAsync(TIdentity id, CancellationToken cancellationToken) => Task.Run(() => collection.FindOne(item => item.ID.Equals(id)), cancellationToken);

        public Task RemoveAsync(TDocument entity, CancellationToken cancellationToken) => Task.Run(() => collection.Delete(item => item.ID.Equals(entity.ID)), cancellationToken);

        public Task RemoveRangeAsync(IEnumerable<TDocument> entities, CancellationToken cancellationToken)
        {
            if (entities == null)
                throw new ArgumentNullException(nameof(entities));

            var ids = entities.Select(item => item.ID).ToArray();

            return Task.Run(() => collection.Delete(item => ids.Contains(item.ID)), cancellationToken);
        }

        public Task UpdateAsync(TDocument entity, CancellationToken cancellationToken) => Task.Run(() => collection.Update(entity), cancellationToken);

        public Task UpdateRangeAsync(IEnumerable<TDocument> entities, CancellationToken cancellationToken) => Task.Run(() => collection.Update(entities), cancellationToken);
    }
}