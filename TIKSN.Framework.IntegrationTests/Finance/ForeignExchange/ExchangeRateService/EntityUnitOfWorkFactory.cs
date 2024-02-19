using System;
using Microsoft.EntityFrameworkCore;
using TIKSN.Data.EntityFrameworkCore;
using TIKSN.Finance.ForeignExchange.Data.EntityFrameworkCore;

namespace TIKSN.IntegrationTests.Finance.ForeignExchange.ExchangeRateService;

public class EntityUnitOfWorkFactory : EntityUnitOfWorkFactoryBase
{
    private readonly ExchangeRatesContext exchangeRatesContext;

    public EntityUnitOfWorkFactory(
        IServiceProvider serviceProvider,
        ExchangeRatesContext exchangeRatesContext)
        : base(serviceProvider)
        => this.exchangeRatesContext = exchangeRatesContext ?? throw new ArgumentNullException(nameof(exchangeRatesContext));

    protected override DbContext[] GetContexts()
        => [this.exchangeRatesContext];
}
