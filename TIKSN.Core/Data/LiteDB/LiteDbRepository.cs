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

        public Task AddAsync(TDocument entity, CancellationToken cancellationToken)
        {
            collection.Insert(entity);
            return Task.CompletedTask;
        }

        public Task AddOrUpdateAsync(TDocument entity, CancellationToken cancellationToken)
        {
            collection.Upsert(entity);
            return Task.CompletedTask;
        }

        public Task AddOrUpdateRangeAsync(IEnumerable<TDocument> entities, CancellationToken cancellationToken)
        {
            collection.Upsert(entities);
            return Task.CompletedTask;
        }

        public Task AddRangeAsync(IEnumerable<TDocument> entities, CancellationToken cancellationToken)
        {
            collection.Insert(entities);
            return Task.CompletedTask;
        }

        public Task<bool> ExistsAsync(TIdentity id, CancellationToken cancellationToken)
        {
            return Task.FromResult(collection.Exists(item => item.ID.Equals(id)));
        }

        public async Task<TDocument> GetAsync(TIdentity id, CancellationToken cancellationToken)
        {
            var result = await GetOrDefaultAsync(id, cancellationToken);

            if (result == null)
            {
                throw new NullReferenceException("Result retrieved from database is null.");
            }

            return result;
        }

        public Task<TDocument> GetOrDefaultAsync(TIdentity id, CancellationToken cancellationToken)
        {
            return Task.FromResult(collection.FindOne(item => item.ID.Equals(id)));
        }

        public Task<IEnumerable<TDocument>> ListAsync(IEnumerable<TIdentity> ids, CancellationToken cancellationToken)
        {
            return Task.FromResult<IEnumerable<TDocument>>(collection.Find(item => ids.Contains(item.ID)).ToArray());
        }

        public Task RemoveAsync(TDocument entity, CancellationToken cancellationToken)
        {
            return Task.FromResult(collection.Delete(item => item.ID.Equals(entity.ID)));
        }

        public Task RemoveRangeAsync(IEnumerable<TDocument> entities, CancellationToken cancellationToken)
        {
            if (entities == null)
            {
                throw new ArgumentNullException(nameof(entities));
            }

            var ids = entities.Select(item => item.ID).ToArray();

            collection.Delete(item => ids.Contains(item.ID));
            return Task.CompletedTask;
        }

        public Task UpdateAsync(TDocument entity, CancellationToken cancellationToken)
        {
            collection.Update(entity);
            return Task.CompletedTask;
        }

        public Task UpdateRangeAsync(IEnumerable<TDocument> entities, CancellationToken cancellationToken)
        {
            collection.Update(entities);
            return Task.CompletedTask;
        }
    }
}