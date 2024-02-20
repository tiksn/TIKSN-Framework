namespace TIKSN.Data;

public interface IUnitOfWork : IAsyncDisposable
{
    IServiceProvider Services { get; }

    Task CompleteAsync(CancellationToken cancellationToken);

    Task DiscardAsync(CancellationToken cancellationToken);
}
