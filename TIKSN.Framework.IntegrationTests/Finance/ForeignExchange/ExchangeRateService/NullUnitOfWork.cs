using System;
using System.Threading;
using System.Threading.Tasks;
using TIKSN.Data;

namespace TIKSN.IntegrationTests.Finance.ForeignExchange.ExchangeRateService;

public class NullUnitOfWork : IUnitOfWork
{
    private bool disposedValue;

    public NullUnitOfWork(IServiceProvider serviceProvider)
        => this.Services = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));

    public IServiceProvider Services { get; }

    public Task CompleteAsync(CancellationToken cancellationToken) => Task.CompletedTask;

    public Task DiscardAsync(CancellationToken cancellationToken) => Task.CompletedTask;

    public void Dispose() => this.Dispose(disposing: true);

    public ValueTask DisposeAsync()
    {
        GC.SuppressFinalize(this);
        return ValueTask.CompletedTask;
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!this.disposedValue)
        {
            this.disposedValue = true;
        }
    }
}
