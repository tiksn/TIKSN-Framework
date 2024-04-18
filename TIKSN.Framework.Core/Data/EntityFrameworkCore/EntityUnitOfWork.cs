using Microsoft.EntityFrameworkCore;

namespace TIKSN.Data.EntityFrameworkCore;

public class EntityUnitOfWork : UnitOfWorkBase
{
    private readonly DbContext[] dbContexts;
    private readonly IServiceProvider services;

    public EntityUnitOfWork(DbContext[] dbContexts, IServiceProvider services)
    {
        this.dbContexts = dbContexts ?? throw new ArgumentNullException(nameof(dbContexts));
        this.services = services ?? throw new ArgumentNullException(nameof(services));
    }

    public override IServiceProvider Services => this.services;

    public override async Task CompleteAsync(CancellationToken cancellationToken)
    {
        var tasks = this.dbContexts.Select(dbContext => dbContext.SaveChangesAsync(cancellationToken)).ToArray();

        _ = await Task.WhenAll(tasks).ConfigureAwait(false);
    }

    public override Task DiscardAsync(CancellationToken cancellationToken)
    {
        _ = this.dbContexts.Do(dbContext => dbContext.ChangeTracker.Clear());

        return Task.CompletedTask;
    }

    protected override bool IsDirty() => Array.Exists(this.dbContexts, dbContext => dbContext.ChangeTracker.HasChanges());
}
