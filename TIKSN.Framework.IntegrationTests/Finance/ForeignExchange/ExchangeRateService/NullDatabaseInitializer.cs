using System.Threading;
using System.Threading.Tasks;

namespace TIKSN.IntegrationTests.Finance.ForeignExchange.ExchangeRateService;

public class NullDatabaseInitializer : IDatabaseInitializer
{
    public Task InitializeAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
