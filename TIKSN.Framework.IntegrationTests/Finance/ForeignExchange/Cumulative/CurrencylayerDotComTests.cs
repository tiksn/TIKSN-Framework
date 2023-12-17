using System;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using TIKSN.DependencyInjection;
using TIKSN.Finance;
using TIKSN.Finance.ForeignExchange.Cumulative;
using Xunit;

namespace TIKSN.IntegrationTests.Finance.ForeignExchange.Cumulative;

public class CurrencylayerDotComTests
{
    private const string Skip = "API changed, code needs to be adopted";

    private readonly ICurrencylayerDotCom exchange;

    public CurrencylayerDotComTests()
    {
        var services = new ServiceCollection();
        _ = services.AddFrameworkCore();
        var serviceProvider = services.BuildServiceProvider();
        this.exchange = serviceProvider.GetRequiredService<ICurrencylayerDotCom>();
    }

    [Fact(Skip = Skip)]
    public async Task GetCurrencyPairs001()
    {
        var pairs = await this.exchange.GetCurrencyPairsAsync(DateTimeOffset.Now, default).ConfigureAwait(true);

        _ = pairs.Any().Should().BeTrue();
    }

    [Fact(Skip = Skip)]
    public async Task GetExchangeRateAsync001()
    {
        var pair = new CurrencyPair(new CurrencyInfo("USD"), new CurrencyInfo("UAH"));

        var rate = await this.exchange.GetExchangeRateAsync(pair, DateTimeOffset.Now, default).ConfigureAwait(true);

        _ = (rate > decimal.Zero).Should().BeTrue();
    }
}
