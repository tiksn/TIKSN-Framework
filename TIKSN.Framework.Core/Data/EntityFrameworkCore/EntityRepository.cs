using Microsoft.EntityFrameworkCore;

namespace TIKSN.Data.EntityFrameworkCore;

public class EntityRepository<TContext, TEntity> : IRepository<TEntity>
    where TContext : DbContext
    where TEntity : class, new()
{
    protected readonly TContext dbContext;

    public EntityRepository(TContext dbContext) =>
        this.dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));

    public Task AddAsync(TEntity entity, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(entity);

        _ = this.dbContext.Add(entity);

        return Task.CompletedTask;
    }

    public Task AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(entities);

        this.dbContext.AddRange(entities);

        return Task.CompletedTask;
    }

    public Task RemoveAsync(TEntity entity, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(entity);

        this.dbContext.Entry(entity).State = EntityState.Deleted;

        return Task.CompletedTask;
    }

    public Task RemoveRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(entities);

        foreach (var entity in entities)
        {
            this.dbContext.Entry(entity).State = EntityState.Deleted;
        }

        return Task.CompletedTask;
    }

    public Task UpdateAsync(TEntity entity, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(entity);

        this.dbContext.Entry(entity).State = EntityState.Modified;

        return Task.CompletedTask;
    }

    public Task UpdateRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(entities);

        foreach (var entity in entities)
        {
            this.dbContext.Entry(entity).State = EntityState.Modified;
        }

        return Task.CompletedTask;
    }
}
