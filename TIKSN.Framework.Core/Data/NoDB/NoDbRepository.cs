using System.Runtime.CompilerServices;
using Microsoft.Extensions.Options;
using NoDb;

namespace TIKSN.Data.NoDB;

public class NoDbRepository<TEntity, TIdentity> : IRepository<TEntity>, IQueryRepository<TEntity, TIdentity>,
    IStreamRepository<TEntity>
    where TEntity : class, IEntity<TIdentity>
    where TIdentity : IEquatable<TIdentity>
{
    private readonly IBasicCommands<TEntity> basicCommands;
    private readonly IBasicQueries<TEntity> basicQueries;
    private readonly string projectId;

    public NoDbRepository(
        IBasicCommands<TEntity> basicCommands,
        IBasicQueries<TEntity> basicQueries,
        IOptions<NoDbRepositoryOptions> genericOptions,
        IOptions<NoDbRepositoryOptions<TEntity>> specificOptions)
    {
        if (genericOptions is null)
        {
            throw new ArgumentNullException(nameof(genericOptions));
        }

        if (specificOptions is null)
        {
            throw new ArgumentNullException(nameof(specificOptions));
        }

        this.basicCommands = basicCommands ?? throw new ArgumentNullException(nameof(basicCommands));
        this.basicQueries = basicQueries ?? throw new ArgumentNullException(nameof(basicQueries));
        this.projectId = string.IsNullOrEmpty(genericOptions.Value.ProjectId)
            ? specificOptions.Value.ProjectId
            : genericOptions.Value.ProjectId;
    }

    public Task AddAsync(TEntity entity, CancellationToken cancellationToken)
    {
        if (entity is null)
        {
            throw new ArgumentNullException(nameof(entity));
        }

        return this.basicCommands.CreateAsync(this.projectId, entity.ID.ToString(), entity, cancellationToken);
    }

    public Task AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken) =>
        BatchOperationHelper.BatchOperationAsync(entities, cancellationToken, this.AddAsync);

    public async Task<bool> ExistsAsync(TIdentity id, CancellationToken cancellationToken) =>
        await this.basicQueries.FetchAsync(this.projectId, id.ToString(), cancellationToken).ConfigureAwait(false) != null;

    public async Task<TEntity> GetAsync(TIdentity id, CancellationToken cancellationToken)
        => await this.GetOrDefaultAsync(id, cancellationToken).ConfigureAwait(false)
            ?? throw new EntityNotFoundException("Result retrieved from database is null.");

    public Task<TEntity> GetOrDefaultAsync(TIdentity id, CancellationToken cancellationToken) =>
        this.basicQueries.FetchAsync(this.projectId, id.ToString(), cancellationToken);

    public async Task<IEnumerable<TEntity>>
        ListAsync(IEnumerable<TIdentity> ids, CancellationToken cancellationToken) =>
        await BatchOperationHelper.BatchOperationAsync(ids, cancellationToken, this.GetAsync).ConfigureAwait(false);

    public async Task<PageResult<TEntity>> PageAsync(
        PageQuery pageQuery,
        CancellationToken cancellationToken)
    {
        var entities = await this.basicQueries
            .GetAllAsync(this.projectId, cancellationToken)
            .ConfigureAwait(false);

        var entitiesQuery = entities.AsQueryable();

        return await PaginationQueryableHelper.PageAsync(
            entitiesQuery,
            pageQuery,
            static (q, ct) => Task.FromResult(q.ToList()),
            static (q, ct) => Task.FromResult(q.LongCount()),
            cancellationToken).ConfigureAwait(false);
    }

    public Task RemoveAsync(TEntity entity, CancellationToken cancellationToken)
    {
        if (entity is null)
        {
            throw new ArgumentNullException(nameof(entity));
        }

        return this.basicCommands.DeleteAsync(this.projectId, entity.ID.ToString(), cancellationToken);
    }

    public Task RemoveRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken) =>
        BatchOperationHelper.BatchOperationAsync(entities, cancellationToken, this.RemoveAsync);

    public async IAsyncEnumerable<TEntity> StreamAllAsync([EnumeratorCancellation] CancellationToken cancellationToken)
    {
        var entities = await this.basicQueries.GetAllAsync(this.projectId, cancellationToken).ConfigureAwait(false);
        foreach (var entity in entities)
        {
            yield return entity;
        }
    }

    public Task UpdateAsync(TEntity entity, CancellationToken cancellationToken)
    {
        if (entity is null)
        {
            throw new ArgumentNullException(nameof(entity));
        }

        return this.basicCommands.UpdateAsync(this.projectId, entity.ID.ToString(), entity, cancellationToken);
    }

    public Task UpdateRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken) =>
        BatchOperationHelper.BatchOperationAsync(entities, cancellationToken, this.UpdateAsync);

    protected Task<IEnumerable<TEntity>> GetAllAsync(CancellationToken cancellationToken) =>
        this.basicQueries.GetAllAsync(this.projectId, cancellationToken);
}
