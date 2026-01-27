using LanguageExt;
using LiteDB;
using static LanguageExt.Prelude;
using Query = LiteDB.Query;

namespace TIKSN.Data.LiteDB;

public class LiteDbRepository<TDocument, TIdentity> : ILiteDbRepository<TDocument, TIdentity>
    where TDocument : IEntity<TIdentity> where TIdentity : IEquatable<TIdentity>
{
    protected LiteDbRepository(
        ILiteDbDatabaseProvider databaseProvider,
        string collectionName,
        Func<TIdentity, BsonValue> convertToBsonValue)
    {
        ArgumentNullException.ThrowIfNull(databaseProvider);

        if (string.IsNullOrEmpty(collectionName))
        {
            throw new ArgumentException($"'{nameof(collectionName)}' cannot be null or empty.", nameof(collectionName));
        }

        var database = databaseProvider.GetDatabase();
        this.Collection = database.GetCollection<TDocument>(collectionName);
        this.ConvertToBsonValue = convertToBsonValue ?? throw new ArgumentNullException(nameof(convertToBsonValue));
    }

    protected ILiteCollection<TDocument> Collection { get; }
    protected Func<TIdentity, BsonValue> ConvertToBsonValue { get; }
    protected virtual Query PageQuery => Query.All();

    public Task AddAsync(TDocument entity, CancellationToken cancellationToken)
    {
        _ = this.Collection.Insert(entity);
        return Task.CompletedTask;
    }

    public Task AddOrUpdateAsync(TDocument entity, CancellationToken cancellationToken)
    {
        _ = this.Collection.Upsert(entity);
        return Task.CompletedTask;
    }

    public Task AddOrUpdateRangeAsync(IEnumerable<TDocument> entities, CancellationToken cancellationToken)
    {
        _ = this.Collection.Upsert(entities);
        return Task.CompletedTask;
    }

    public Task AddRangeAsync(IEnumerable<TDocument> entities, CancellationToken cancellationToken)
    {
        _ = this.Collection.Insert(entities);
        return Task.CompletedTask;
    }

    public Task<bool> ExistsAsync(TIdentity id, CancellationToken cancellationToken) =>
        Task.FromResult(this.Collection.Exists(item => item.ID.Equals(id)));

    public async Task<TDocument> GetAsync(TIdentity id, CancellationToken cancellationToken)
        => await this.GetOrDefaultAsync(id, cancellationToken).ConfigureAwait(false)
            ?? throw new EntityNotFoundException("Result retrieved from database is null.");

    public Task<TDocument?> GetOrDefaultAsync(TIdentity id, CancellationToken cancellationToken)
        => Task.FromResult(this.Collection.FindById(this.ConvertToBsonValue(id)));

    public Task<IReadOnlyList<TDocument>> ListAsync(
        IEnumerable<TIdentity> ids,
        CancellationToken cancellationToken)
        => Task.FromResult<IReadOnlyList<TDocument>>([.. this.Collection.Find(item => ids.Contains(item.ID))]);

    public Task<PageResult<TDocument>> PageAsync(
        PageQuery pageQuery,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(pageQuery);

        var items = this.Collection.Find(
            this.PageQuery,
            pageQuery.Page.Index * pageQuery.Page.Size,
            pageQuery.Page.Size)
            .ToArray();

        Option<long> totalItems = pageQuery.EstimateTotalItems
            ? this.Collection.LongCount(this.PageQuery)
            : None;

        return Task.FromResult(new PageResult<TDocument>(pageQuery.Page, items, totalItems));
    }

    public Task RemoveAsync(TDocument entity, CancellationToken cancellationToken) =>
        Task.FromResult(this.Collection.Delete(this.ConvertToBsonValue(entity.ID)));

    public Task RemoveRangeAsync(IEnumerable<TDocument> entities, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(entities);

        var ids = entities.Select(item => item.ID).ToArray();

        _ = this.Collection.DeleteMany(item => ids.Contains(item.ID));
        return Task.CompletedTask;
    }

    public IAsyncEnumerable<TDocument> StreamAllAsync(CancellationToken cancellationToken) =>
        this.Collection.FindAll().ToAsyncEnumerable();

    public Task UpdateAsync(TDocument entity, CancellationToken cancellationToken)
    {
        _ = this.Collection.Update(entity);
        return Task.CompletedTask;
    }

    public Task UpdateRangeAsync(IEnumerable<TDocument> entities, CancellationToken cancellationToken)
    {
        _ = this.Collection.Update(entities);
        return Task.CompletedTask;
    }
}
