namespace TIKSN.Data;

public interface IUnitOfWork : IDisposable, IAsyncDisposable
{
    public IServiceProvider Services { get; }

    Task CompleteAsync(CancellationToken cancellationToken);

    Task DiscardAsync(CancellationToken cancellationToken);
}
