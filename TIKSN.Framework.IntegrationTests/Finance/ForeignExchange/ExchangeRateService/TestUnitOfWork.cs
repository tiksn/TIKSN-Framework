using System.Threading;
using System.Threading.Tasks;
using TIKSN.Data;

namespace TIKSN.Finance.ForeignExchange.ExchangeRateService.IntegrationTests
{
    public class TestUnitOfWork : IUnitOfWork
    {
        public Task CompleteAsync(CancellationToken cancellationToken) => Task.CompletedTask;

        public Task DiscardAsync(CancellationToken cancellationToken) => Task.CompletedTask;

        public void Dispose()
        { }

        public ValueTask DisposeAsync() => ValueTask.CompletedTask;
    }
}
