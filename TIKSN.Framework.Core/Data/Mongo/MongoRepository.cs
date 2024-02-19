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
        Task NoneAsync() =>
            this.Collection.InsertOneAsync(entity, options: null, cancellationToken);

        Task SomeAsync(IClientSessionHandle clientSessionHandle) =>
            this.Collection.InsertOneAsync(clientSessionHandle, entity, options: null, cancellationToken);

        return this.MongoClientSessionProvider.GetClientSessionHandle().Match(SomeAsync, NoneAsync);
    }

    public Task AddOrUpdateAsync(TDocument entity, CancellationToken cancellationToken)
    {
        var replaceOptions = new ReplaceOptions { IsUpsert = true };

        Task NoneAsync() => this.Collection.ReplaceOneAsync(item => item.ID.Equals(entity.ID), entity, replaceOptions,
                cancellationToken);

        Task SomeAsync(IClientSessionHandle clientSessionHandle) => this.Collection.ReplaceOneAsync(clientSessionHandle, item => item.ID.Equals(entity.ID), entity,
                replaceOptions, cancellationToken);

        return this.MongoClientSessionProvider.GetClientSessionHandle().Match(SomeAsync, NoneAsync);
    }

    public Task AddRangeAsync(IEnumerable<TDocument> entities, CancellationToken cancellationToken)
    {
        Task NoneAsync() => this.Collection.InsertManyAsync(entities, cancellationToken: cancellationToken);

        Task SomeAsync(IClientSessionHandle clientSessionHandle) => this.Collection.InsertManyAsync(clientSessionHandle, entities,
                cancellationToken: cancellationToken);

        return this.MongoClientSessionProvider.GetClientSessionHandle().Match(SomeAsync, NoneAsync);
    }

    public Task<bool> ExistsAsync(TIdentity id, CancellationToken cancellationToken)
    {
        Task<bool> NoneAsync() => this.Collection.Find(GetIdentityFilter(id)).AnyAsync(cancellationToken);

        Task<bool> SomeAsync(IClientSessionHandle clientSessionHandle) => this.Collection.Find(clientSessionHandle, GetIdentityFilter(id)).AnyAsync(cancellationToken);

        return this.MongoClientSessionProvider.GetClientSessionHandle().Match(SomeAsync, NoneAsync);
    }

    public Task<TDocument> GetAsync(TIdentity id, CancellationToken cancellationToken)
    {
        Task<TDocument> NoneAsync() => this.Collection.Find(GetIdentityFilter(id)).SingleAsync(cancellationToken);

        Task<TDocument> SomeAsync(IClientSessionHandle clientSessionHandle) => this.Collection.Find(clientSessionHandle, GetIdentityFilter(id)).SingleAsync(cancellationToken);

        return this.MongoClientSessionProvider.GetClientSessionHandle().Match(SomeAsync, NoneAsync);
    }

    public Task<TDocument> GetOrDefaultAsync(TIdentity id, CancellationToken cancellationToken)
    {
        Task<TDocument> NoneAsync() => this.Collection.Find(GetIdentityFilter(id)).SingleOrDefaultAsync(cancellationToken);

        Task<TDocument> SomeAsync(IClientSessionHandle clientSessionHandle) => this.Collection.Find(clientSessionHandle, GetIdentityFilter(id))
                .SingleOrDefaultAsync(cancellationToken);

        return this.MongoClientSessionProvider.GetClientSessionHandle().Match(SomeAsync, NoneAsync);
    }

    public async Task<IEnumerable<TDocument>> ListAsync(IEnumerable<TIdentity> ids,
        CancellationToken cancellationToken)
    {
        async Task<IEnumerable<TDocument>> NoneAsync() => await this.Collection.Find(GetIdentitiesFilter(ids)).ToListAsync(cancellationToken).ConfigureAwait(false);

        async Task<IEnumerable<TDocument>> SomeAsync(IClientSessionHandle clientSessionHandle) => await this.Collection.Find(clientSessionHandle, GetIdentitiesFilter(ids))
                .ToListAsync(cancellationToken).ConfigureAwait(false);

        return await this.MongoClientSessionProvider.GetClientSessionHandle().Match(SomeAsync, NoneAsync).ConfigureAwait(false);
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
        Task NoneAsync() => this.Collection.DeleteOneAsync(item => item.ID.Equals(entity.ID), cancellationToken);

        Task SomeAsync(IClientSessionHandle clientSessionHandle) => this.Collection.DeleteOneAsync(clientSessionHandle, item => item.ID.Equals(entity.ID),
                cancellationToken: cancellationToken);

        return this.MongoClientSessionProvider.GetClientSessionHandle().Match(SomeAsync, NoneAsync);
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

        Task NoneAsync() => this.Collection.DeleteManyAsync(filter, cancellationToken);

        Task SomeAsync(IClientSessionHandle clientSessionHandle) =>
            this.Collection.DeleteManyAsync(clientSessionHandle, filter, options: null, cancellationToken);

        return this.MongoClientSessionProvider.GetClientSessionHandle().Match(SomeAsync, NoneAsync);
    }

    public async IAsyncEnumerable<TDocument> StreamAllAsync([EnumeratorCancellation] CancellationToken cancellationToken)
    {
        Task<IAsyncCursor<TDocument>> NoneAsync() => this.Collection.Find(FilterDefinition<TDocument>.Empty).ToCursorAsync(cancellationToken);

        Task<IAsyncCursor<TDocument>> SomeAsync(IClientSessionHandle clientSessionHandle) => this.Collection.Find(clientSessionHandle, FilterDefinition<TDocument>.Empty)
                .ToCursorAsync(cancellationToken);

        var cursor = await this.MongoClientSessionProvider.GetClientSessionHandle().Match(SomeAsync, NoneAsync).ConfigureAwait(false);

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
        Task NoneAsync() => this.Collection.ReplaceOneAsync(item => item.ID.Equals(entity.ID), entity,
                cancellationToken: cancellationToken);

        Task SomeAsync(IClientSessionHandle clientSessionHandle) => this.Collection.ReplaceOneAsync(clientSessionHandle, item => item.ID.Equals(entity.ID), entity,
                cancellationToken: cancellationToken);

        return this.MongoClientSessionProvider.GetClientSessionHandle().Match(SomeAsync, NoneAsync);
    }

    public Task UpdateRangeAsync(IEnumerable<TDocument> entities, CancellationToken cancellationToken) =>
        BatchOperationHelper.BatchOperationAsync(entities, this.UpdateAsync, cancellationToken);

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

        Task<List<TDocument>> ItemsNoneAsync() => this.Collection.Find(filter)
            .Skip(pageQuery.Page.Index * pageQuery.Page.Size)
            .Limit(pageQuery.Page.Size)
            .ToListAsync(cancellationToken);

        Task<List<TDocument>> ItemsSomeAsync(IClientSessionHandle clientSessionHandle) => this.Collection.Find(clientSessionHandle, filter)
            .Skip(pageQuery.Page.Index * pageQuery.Page.Size)
            .Limit(pageQuery.Page.Size)
            .ToListAsync(cancellationToken);

        Task<long> CountNoneAsync() => this.Collection.Find(filter)
            .CountDocumentsAsync(cancellationToken);

        Task<long> CountSomeAsync(IClientSessionHandle clientSessionHandle) => this.Collection.Find(clientSessionHandle, filter)
            .CountDocumentsAsync(cancellationToken);

        var items = await this.MongoClientSessionProvider.GetClientSessionHandle().Match(ItemsSomeAsync, ItemsNoneAsync).ConfigureAwait(false);

        Option<long> totalItems = pageQuery.EstimateTotalItems
            ? await this.MongoClientSessionProvider.GetClientSessionHandle().Match(CountSomeAsync, CountNoneAsync).ConfigureAwait(false)
            : None;

        return new PageResult<TDocument>(pageQuery.Page, items, totalItems);
    }

    protected async Task<IReadOnlyList<TDocument>> SearchAsync(
        FilterDefinition<TDocument> filter,
        CancellationToken cancellationToken)
    {
        Task<List<TDocument>> NoneAsync() => this.Collection.Find(filter)
            .ToListAsync(cancellationToken);

        Task<List<TDocument>> SomeAsync(IClientSessionHandle clientSessionHandle) => this.Collection.Find(clientSessionHandle, filter)
            .ToListAsync(cancellationToken);

        return await this.MongoClientSessionProvider.GetClientSessionHandle().Match(SomeAsync, NoneAsync).ConfigureAwait(false);
    }

    protected Task<TDocument> SingleOrDefaultAsync(
        FilterDefinition<TDocument> filter,
        CancellationToken cancellationToken)
    {
        Task<TDocument> NoneAsync() => this.Collection.Find(filter)
            .SingleOrDefaultAsync(cancellationToken);

        Task<TDocument> SomeAsync(IClientSessionHandle clientSessionHandle) => this.Collection.Find(clientSessionHandle, filter)
            .SingleOrDefaultAsync(cancellationToken);

        return this.MongoClientSessionProvider.GetClientSessionHandle().Match(SomeAsync, NoneAsync);
    }
}
