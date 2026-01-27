using System.Threading;
using System.Threading.Tasks;

namespace TIKSN.IntegrationTests.Finance.ForeignExchange.ExchangeRateService;

public interface IDatabaseInitializer
{
    public Task InitializeAsync(CancellationToken cancellationToken);
}
