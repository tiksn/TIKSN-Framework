using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using TIKSN.Finance.ForeignExchange.Cumulative;
using TIKSN.Globalization;
using TIKSN.IntegrationTests;
using Xunit;

namespace TIKSN.Finance.ForeignExchange.IntegrationTests;

[Collection("ServiceProviderCollection")]
public class MyCurrencyDotNetTests
{
    private readonly TimeProvider timeProvider;
    private readonly IHttpClientFactory httpClientFactory;
    private readonly ServiceProviderFixture serviceProviderFixture;

    public MyCurrencyDotNetTests(ServiceProviderFixture serviceProviderFixture)
    {
        this.timeProvider = serviceProviderFixture.GetServiceProvider().GetRequiredService<TimeProvider>();
        this.httpClientFactory = serviceProviderFixture.GetServiceProvider().GetRequiredService<IHttpClientFactory>();
        this.serviceProviderFixture = serviceProviderFixture ?? throw new ArgumentNullException(nameof(serviceProviderFixture));
    }

    [Fact]
    public async Task GetCurrencyPairsAsync()
    {
        var currencyFactory = this.serviceProviderFixture.GetServiceProvider().GetRequiredService<ICurrencyFactory>();

        var myCurrencyDotNet = new MyCurrencyDotNet(this.httpClientFactory, currencyFactory, this.timeProvider);

        var pairs = await myCurrencyDotNet.GetCurrencyPairsAsync(this.timeProvider.GetUtcNow(), default).ConfigureAwait(true);

        _ = pairs.Count().Should().BeGreaterThan(0);
    }

    [Fact]
    public async Task GetExchangeRateAsync001Async()
    {
        var currencyFactory = this.serviceProviderFixture.GetServiceProvider().GetRequiredService<ICurrencyFactory>();

        var myCurrencyDotNet = new MyCurrencyDotNet(this.httpClientFactory, currencyFactory, this.timeProvider);

        var amd = currencyFactory.Create("AMD");
        var usd = currencyFactory.Create("USD");
        var pair = new CurrencyPair(usd, amd);

        var rate = await myCurrencyDotNet.GetExchangeRateAsync(pair, this.timeProvider.GetUtcNow(), default).ConfigureAwait(true);

        _ = rate.Should().BeGreaterThan(decimal.One);
    }

    [Fact]
    public async Task GetExchangeRateAsync002Async()
    {
        var currencyFactory = this.serviceProviderFixture.GetServiceProvider().GetRequiredService<ICurrencyFactory>();

        var myCurrencyDotNet = new MyCurrencyDotNet(this.httpClientFactory, currencyFactory, this.timeProvider);

        var amd = currencyFactory.Create("AMD");
        var usd = currencyFactory.Create("USD");
        var pair = new CurrencyPair(amd, usd);

        var rate = await myCurrencyDotNet.GetExchangeRateAsync(pair, this.timeProvider.GetUtcNow(), default).ConfigureAwait(true);

        _ = rate.Should().BeLessThan(decimal.One);
    }
}
