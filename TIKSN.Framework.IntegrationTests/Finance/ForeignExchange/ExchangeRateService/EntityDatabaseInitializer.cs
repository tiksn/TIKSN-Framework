using System;
using System.Threading;
using System.Threading.Tasks;
using TIKSN.Finance.ForeignExchange.Data.EntityFrameworkCore;

namespace TIKSN.IntegrationTests.Finance.ForeignExchange.ExchangeRateService;

public class EntityDatabaseInitializer : IDatabaseInitializer
{
    private readonly ExchangeRatesContext exchangeRatesContext;

    public EntityDatabaseInitializer(ExchangeRatesContext exchangeRatesContext)
        => this.exchangeRatesContext = exchangeRatesContext ?? throw new ArgumentNullException(nameof(exchangeRatesContext));

    public Task InitializeAsync(CancellationToken cancellationToken)
        => this.exchangeRatesContext.Database.EnsureCreatedAsync(cancellationToken);
}
