using System.Runtime.CompilerServices;
using Microsoft.EntityFrameworkCore;

namespace TIKSN.Data.EntityFrameworkCore;

public abstract class EntityQueryRepository<TContext, TEntity, TIdentity> : EntityRepository<TContext, TEntity>,
    IQueryRepository<TEntity, TIdentity>, IStreamRepository<TEntity>
    where TContext : DbContext
    where TEntity : class, IEntity<TIdentity>, new()
    where TIdentity : IEquatable<TIdentity>
{
    protected EntityQueryRepository(TContext dbContext) : base(dbContext)
    {
    }

    protected virtual IQueryable<TEntity> Entities => this.DbContext.Set<TEntity>().AsNoTracking();

    protected abstract IOrderedQueryable<TEntity> OrderedEntities { get; }

    public Task<bool> ExistsAsync(TIdentity id, CancellationToken cancellationToken) =>
        this.Entities.AnyAsync(a => a.ID.Equals(id), cancellationToken);

    public Task<TEntity> GetAsync(TIdentity id, CancellationToken cancellationToken) =>
        this.Entities.SingleAsync(entity => entity.ID.Equals(id), cancellationToken);

    public Task<TEntity?> GetOrDefaultAsync(TIdentity id, CancellationToken cancellationToken) =>
        this.Entities.SingleOrDefaultAsync(entity => entity.ID.Equals(id), cancellationToken);

    public async Task<IReadOnlyList<TEntity>> ListAsync(
        IEnumerable<TIdentity> ids,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(ids);

        return await this.Entities.Where(entity => ids.Contains(entity.ID)).ToListAsync(cancellationToken).ConfigureAwait(false);
    }

    public Task<PageResult<TEntity>> PageAsync(
        PageQuery pageQuery,
        CancellationToken cancellationToken)
        => PageAsync(
            this.OrderedEntities,
            pageQuery,
            cancellationToken);

    public async IAsyncEnumerable<TEntity> StreamAllAsync([EnumeratorCancellation] CancellationToken cancellationToken)
    {
        await foreach (var entity in this.Entities.AsAsyncEnumerable().WithCancellation(cancellationToken).ConfigureAwait(false))
        {
            yield return entity;
        }
    }

    protected static Task<PageResult<TEntity>> PageAsync(
        IOrderedQueryable<TEntity> query,
        PageQuery pageQuery,
        CancellationToken cancellationToken)
        => PaginationQueryableHelper.PageAsync(
            query,
            pageQuery,
            static (q, ct) => q.ToListAsync(ct),
            static (q, ct) => q.LongCountAsync(ct),
            cancellationToken);
}
