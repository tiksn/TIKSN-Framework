namespace TIKSN.Data;

public interface IUnitOfWork : IAsyncDisposable
{
    public IServiceProvider Services { get; }

    public Task CompleteAsync(CancellationToken cancellationToken);

    public Task DiscardAsync(CancellationToken cancellationToken);
}
