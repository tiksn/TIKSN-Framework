namespace TIKSN.Data;

public abstract class UnitOfWorkBase : IUnitOfWork
{
    public abstract IServiceProvider Services { get; }

    public abstract Task CompleteAsync(CancellationToken cancellationToken);

    public abstract Task DiscardAsync(CancellationToken cancellationToken);

    public virtual async ValueTask DisposeAsync()
    {
        if (this.IsDirty())
        {
            await this.DiscardAsync(default).ConfigureAwait(false);
        }
        GC.SuppressFinalize(this);
    }

    protected abstract bool IsDirty();
}
