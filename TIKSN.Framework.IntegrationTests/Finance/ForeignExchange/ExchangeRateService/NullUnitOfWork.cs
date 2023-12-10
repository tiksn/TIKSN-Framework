using System;
using System.Threading;
using System.Threading.Tasks;
using TIKSN.Data;

namespace TIKSN.Finance.ForeignExchange.ExchangeRateService.IntegrationTests;

public class NullUnitOfWork : IUnitOfWork
{
    private readonly IServiceProvider serviceProvider;

    public NullUnitOfWork(IServiceProvider serviceProvider)
        => this.serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));

    public IServiceProvider Services => this.serviceProvider;

    public Task CompleteAsync(CancellationToken cancellationToken) => Task.CompletedTask;

    public Task DiscardAsync(CancellationToken cancellationToken) => Task.CompletedTask;

    public void Dispose()
    { }

    public ValueTask DisposeAsync() => ValueTask.CompletedTask;
}
