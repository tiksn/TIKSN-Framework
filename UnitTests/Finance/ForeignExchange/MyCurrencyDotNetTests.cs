using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Threading.Tasks;
using TIKSN.DependencyInjection.Tests;
using TIKSN.Finance.ForeignExchange.Cumulative;
using TIKSN.Globalization;
using Xunit;
using Xunit.Abstractions;

namespace TIKSN.Finance.ForeignExchange.Tests
{
    public class MyCurrencyDotNetTests
    {
        private readonly IServiceProvider _serviceProvider;

        public MyCurrencyDotNetTests(ITestOutputHelper testOutputHelper)
        {
            _serviceProvider = new TestCompositionRootSetup(testOutputHelper, services =>
            {
                services.AddSingleton<ICurrencyFactory, CurrencyFactory>();
                services.AddSingleton<IRegionFactory, RegionFactory>();
            }).CreateServiceProvider();
        }

        [Fact]
        public async Task GetCurrencyPairsAsync()
        {
            var currencyFactory = _serviceProvider.GetRequiredService<ICurrencyFactory>();
            var regionFactory = _serviceProvider.GetRequiredService<IRegionFactory>();

            var myCurrencyDotNet = new MyCurrencyDotNet(currencyFactory, regionFactory);

            var pairs = await myCurrencyDotNet.GetCurrencyPairsAsync(DateTimeOffset.Now, default);

            pairs.Count().Should().BeGreaterThan(0);
        }

        [Fact]
        public async Task GetExchangeRateAsync001()
        {
            var currencyFactory = _serviceProvider.GetRequiredService<ICurrencyFactory>();
            var regionFactory = _serviceProvider.GetRequiredService<IRegionFactory>();

            var myCurrencyDotNet = new MyCurrencyDotNet(currencyFactory, regionFactory);

            var amd = currencyFactory.Create("AMD");
            var usd = currencyFactory.Create("USD");
            var pair = new CurrencyPair(usd, amd);

            var rate = await myCurrencyDotNet.GetExchangeRateAsync(pair, DateTimeOffset.Now, default);

            rate.Should().BeGreaterThan(decimal.One);
        }

        [Fact]
        public async Task GetExchangeRateAsync002()
        {
            var currencyFactory = _serviceProvider.GetRequiredService<ICurrencyFactory>();
            var regionFactory = _serviceProvider.GetRequiredService<IRegionFactory>();

            var myCurrencyDotNet = new MyCurrencyDotNet(currencyFactory, regionFactory);

            var amd = currencyFactory.Create("AMD");
            var usd = currencyFactory.Create("USD");
            var pair = new CurrencyPair(amd, usd);

            var rate = await myCurrencyDotNet.GetExchangeRateAsync(pair, DateTimeOffset.Now, default);

            rate.Should().BeLessThan(decimal.One);
        }
    }
}