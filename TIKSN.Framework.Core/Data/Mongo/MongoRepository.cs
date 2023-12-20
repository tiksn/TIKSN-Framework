using System.Runtime.CompilerServices;
using LanguageExt;
using MongoDB.Driver;
using static LanguageExt.Prelude;

namespace TIKSN.Data.Mongo;

public class MongoRepository<TDocument, TIdentity> : IMongoRepository<TDocument, TIdentity>
    where TDocument : IEntity<TIdentity> where TIdentity : IEquatable<TIdentity>
{
    protected MongoRepository(
        IMongoClientSessionProvider mongoClientSessionProvider,
        IMongoDatabaseProvider mongoDatabaseProvider, string collectionName)
    {
        ArgumentNullException.ThrowIfNull(mongoDatabaseProvider);

        if (string.IsNullOrEmpty(collectionName))
        {
            throw new ArgumentException($"'{nameof(collectionName)}' cannot be null or empty.", nameof(collectionName));
        }

        this.MongoClientSessionProvider = mongoClientSessionProvider ??
                                          throw new ArgumentNullException(nameof(mongoClientSessionProvider));
        var database = mongoDatabaseProvider.GetDatabase();
        this.Collection = database.GetCollection<TDocument>(collectionName);
    }

    protected IMongoCollection<TDocument> Collection { get; }
    protected IMongoClientSessionProvider MongoClientSessionProvider { get; }

    public Task AddAsync(TDocument entity, CancellationToken cancellationToken)
    {
        Task None() =>
            this.Collection.InsertOneAsync(entity, options: null, cancellationToken);

        Task Some(IClientSessionHandle clientSessionHandle) =>
            this.Collection.InsertOneAsync(clientSessionHandle, entity, options: null, cancellationToken);

        return this.MongoClientSessionProvider.GetClientSessionHandle().Match(Some, None);
    }

    public Task AddOrUpdateAsync(TDocument entity, CancellationToken cancellationToken)
    {
        var replaceOptions = new ReplaceOptions { IsUpsert = true };

        Task None() => this.Collection.ReplaceOneAsync(item => item.ID.Equals(entity.ID), entity, replaceOptions,
                cancellationToken);

        Task Some(IClientSessionHandle clientSessionHandle) => this.Collection.ReplaceOneAsync(clientSessionHandle, item => item.ID.Equals(entity.ID), entity,
                replaceOptions, cancellationToken);

        return this.MongoClientSessionProvider.GetClientSessionHandle().Match(Some, None);
    }

    public Task AddRangeAsync(IEnumerable<TDocument> entities, CancellationToken cancellationToken)
    {
        Task None() => this.Collection.InsertManyAsync(entities, cancellationToken: cancellationToken);

        Task Some(IClientSessionHandle clientSessionHandle) => this.Collection.InsertManyAsync(clientSessionHandle, entities,
                cancellationToken: cancellationToken);

        return this.MongoClientSessionProvider.GetClientSessionHandle().Match(Some, None);
    }

    public Task<bool> ExistsAsync(TIdentity id, CancellationToken cancellationToken)
    {
        Task<bool> None() => this.Collection.Find(GetIdentityFilter(id)).AnyAsync(cancellationToken);

        Task<bool> Some(IClientSessionHandle clientSessionHandle) => this.Collection.Find(clientSessionHandle, GetIdentityFilter(id)).AnyAsync(cancellationToken);

        return this.MongoClientSessionProvider.GetClientSessionHandle().Match(Some, None);
    }

    public Task<TDocument> GetAsync(TIdentity id, CancellationToken cancellationToken)
    {
        Task<TDocument> None() => this.Collection.Find(GetIdentityFilter(id)).SingleAsync(cancellationToken);

        Task<TDocument> Some(IClientSessionHandle clientSessionHandle) => this.Collection.Find(clientSessionHandle, GetIdentityFilter(id)).SingleAsync(cancellationToken);

        return this.MongoClientSessionProvider.GetClientSessionHandle().Match(Some, None);
    }

    public Task<TDocument> GetOrDefaultAsync(TIdentity id, CancellationToken cancellationToken)
    {
        Task<TDocument> None() => this.Collection.Find(GetIdentityFilter(id)).SingleOrDefaultAsync(cancellationToken);

        Task<TDocument> Some(IClientSessionHandle clientSessionHandle) => this.Collection.Find(clientSessionHandle, GetIdentityFilter(id))
                .SingleOrDefaultAsync(cancellationToken);

        return this.MongoClientSessionProvider.GetClientSessionHandle().Match(Some, None);
    }

    public async Task<IEnumerable<TDocument>> ListAsync(IEnumerable<TIdentity> ids,
        CancellationToken cancellationToken)
    {
        async Task<IEnumerable<TDocument>> None() => await this.Collection.Find(GetIdentitiesFilter(ids)).ToListAsync(cancellationToken).ConfigureAwait(false);

        async Task<IEnumerable<TDocument>> Some(IClientSessionHandle clientSessionHandle) => await this.Collection.Find(clientSessionHandle, GetIdentitiesFilter(ids))
                .ToListAsync(cancellationToken).ConfigureAwait(false);

        return await this.MongoClientSessionProvider.GetClientSessionHandle().Match(Some, None).ConfigureAwait(false);
    }

    public Task<PageResult<TDocument>> PageAsync(
        PageQuery pageQuery,
        CancellationToken cancellationToken)
        => this.PageAsync(
            FilterDefinition<TDocument>.Empty,
            pageQuery,
            cancellationToken);

    public Task RemoveAsync(TDocument entity, CancellationToken cancellationToken)
    {
        Task None() => this.Collection.DeleteOneAsync(item => item.ID.Equals(entity.ID), cancellationToken);

        Task Some(IClientSessionHandle clientSessionHandle) => this.Collection.DeleteOneAsync(clientSessionHandle, item => item.ID.Equals(entity.ID),
                cancellationToken: cancellationToken);

        return this.MongoClientSessionProvider.GetClientSessionHandle().Match(Some, None);
    }

    public Task RemoveRangeAsync(IEnumerable<TDocument> entities, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(entities);

        var filters = entities.Select(item => GetIdentityFilter(item.ID)).ToArray();

        if (filters.Length == 0)
        {
            return Task.FromResult<object>(null);
        }

        var filter = Builders<TDocument>.Filter.Or(filters);

        Task None() => this.Collection.DeleteManyAsync(filter, cancellationToken);

        Task Some(IClientSessionHandle clientSessionHandle) =>
            this.Collection.DeleteManyAsync(clientSessionHandle, filter, options: null, cancellationToken);

        return this.MongoClientSessionProvider.GetClientSessionHandle().Match(Some, None);
    }

    public async IAsyncEnumerable<TDocument> StreamAllAsync([EnumeratorCancellation] CancellationToken cancellationToken)
    {
        Task<IAsyncCursor<TDocument>> None() => this.Collection.Find(FilterDefinition<TDocument>.Empty).ToCursorAsync(cancellationToken);

        Task<IAsyncCursor<TDocument>> Some(IClientSessionHandle clientSessionHandle) => this.Collection.Find(clientSessionHandle, FilterDefinition<TDocument>.Empty)
                .ToCursorAsync(cancellationToken);

        var cursor = await this.MongoClientSessionProvider.GetClientSessionHandle().Match(Some, None).ConfigureAwait(false);

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
        Task None() => this.Collection.ReplaceOneAsync(item => item.ID.Equals(entity.ID), entity,
                cancellationToken: cancellationToken);

        Task Some(IClientSessionHandle clientSessionHandle) => this.Collection.ReplaceOneAsync(clientSessionHandle, item => item.ID.Equals(entity.ID), entity,
                cancellationToken: cancellationToken);

        return this.MongoClientSessionProvider.GetClientSessionHandle().Match(Some, None);
    }

    public Task UpdateRangeAsync(IEnumerable<TDocument> entities, CancellationToken cancellationToken) =>
        BatchOperationHelper.BatchOperationAsync(entities, cancellationToken, this.UpdateAsync);

    protected static FilterDefinition<TDocument> GetIdentitiesFilter(IEnumerable<TIdentity> ids) =>
        Builders<TDocument>.Filter.In(item => item.ID, ids);

    protected static FilterDefinition<TDocument> GetIdentityFilter(TIdentity id) =>
        Builders<TDocument>.Filter.Eq(item => item.ID, id);

    protected async Task<PageResult<TDocument>> PageAsync(
        FilterDefinition<TDocument> filter,
        PageQuery pageQuery,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(filter);

        ArgumentNullException.ThrowIfNull(pageQuery);

        Task<List<TDocument>> ItemsNone() => this.Collection.Find(filter)
            .Skip(pageQuery.Page.Index * pageQuery.Page.Size)
            .Limit(pageQuery.Page.Size)
            .ToListAsync(cancellationToken);

        Task<List<TDocument>> ItemsSome(IClientSessionHandle clientSessionHandle) => this.Collection.Find(clientSessionHandle, filter)
            .Skip(pageQuery.Page.Index * pageQuery.Page.Size)
            .Limit(pageQuery.Page.Size)
            .ToListAsync(cancellationToken);

        Task<long> CountNone() => this.Collection.Find(filter)
            .CountDocumentsAsync(cancellationToken);

        Task<long> CountSome(IClientSessionHandle clientSessionHandle) => this.Collection.Find(clientSessionHandle, filter)
            .CountDocumentsAsync(cancellationToken);

        var items = await this.MongoClientSessionProvider.GetClientSessionHandle().Match(ItemsSome, ItemsNone).ConfigureAwait(false);

        Option<long> totalItems = pageQuery.EstimateTotalItems
            ? await this.MongoClientSessionProvider.GetClientSessionHandle().Match(CountSome, CountNone).ConfigureAwait(false)
            : None;

        return new PageResult<TDocument>(pageQuery.Page, items, totalItems);
    }

    protected async Task<IReadOnlyList<TDocument>> SearchAsync(
        FilterDefinition<TDocument> filter,
        CancellationToken cancellationToken)
    {
        Task<List<TDocument>> None() => this.Collection.Find(filter)
            .ToListAsync(cancellationToken);

        Task<List<TDocument>> Some(IClientSessionHandle clientSessionHandle) => this.Collection.Find(clientSessionHandle, filter)
            .ToListAsync(cancellationToken);

        return await this.MongoClientSessionProvider.GetClientSessionHandle().Match(Some, None).ConfigureAwait(false);
    }

    protected Task<TDocument> SingleOrDefaultAsync(
        FilterDefinition<TDocument> filter,
        CancellationToken cancellationToken)
    {
        Task<TDocument> None() => this.Collection.Find(filter)
            .SingleOrDefaultAsync(cancellationToken);

        Task<TDocument> Some(IClientSessionHandle clientSessionHandle) => this.Collection.Find(clientSessionHandle, filter)
            .SingleOrDefaultAsync(cancellationToken);

        return this.MongoClientSessionProvider.GetClientSessionHandle().Match(Some, None);
    }
}
