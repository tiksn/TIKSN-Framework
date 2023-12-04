using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using TIKSN.DependencyInjection;
using Xunit;

namespace TIKSN.Finance.ForeignExchange.Cumulative.IntegrationTests;

public class CurrencylayerDotComTests
{
    private const string skip = "API changed, code needs to be adopted";

    private readonly string accessKey = "<put your access key here>";
    private readonly ICurrencylayerDotCom exchange;

    public CurrencylayerDotComTests()
    {
        var services = new ServiceCollection();
        _ = services.AddFrameworkCore();
        var serviceProvider = services.BuildServiceProvider();
        this.exchange = serviceProvider.GetRequiredService<ICurrencylayerDotCom>();
    }

    [Fact(Skip = skip)]
    public async Task GetCurrencyPairs001Async()
    {
        var pairs = await this.exchange.GetCurrencyPairsAsync(DateTimeOffset.Now, default).ConfigureAwait(true);

        Assert.True(pairs.Count() > 0);
    }

    [Fact(Skip = skip)]
    public async Task GetExchangeRateAsync001Async()
    {
        var pair = new CurrencyPair(new CurrencyInfo("USD"), new CurrencyInfo("UAH"));

        var rate = await this.exchange.GetExchangeRateAsync(pair, DateTimeOffset.Now, default).ConfigureAwait(true);

        Assert.True(rate > decimal.Zero);
    }
}
