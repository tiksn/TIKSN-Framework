using System;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TIKSN.Finance.ForeignExchange.Cumulative;
using TIKSN.Framework.IntegrationTests;
using TIKSN.Globalization;
using TIKSN.Time;
using Xunit;

namespace TIKSN.Finance.ForeignExchange.Tests
{
    [Collection("ServiceProviderCollection")]
    public class CurrencyConverterApiDotComTests
    {
        private readonly ITimeProvider timeProvider;
        private readonly string currencyConverterApiKey;
        private readonly ServiceProviderFixture serviceProviderFixture;

        public CurrencyConverterApiDotComTests(ServiceProviderFixture serviceProviderFixture)
        {
            this.timeProvider = serviceProviderFixture.Services.GetRequiredService<ITimeProvider>();
            this.currencyConverterApiKey = serviceProviderFixture.Services.GetRequiredService<IConfiguration>().GetValue<string>("CurrencyConverterApiKey");
            this.serviceProviderFixture = serviceProviderFixture;
        }

        [Fact]
        public async Task GetCurrencyPairsAsync()
        {
            var currencyFactory = this.serviceProviderFixture.Services.GetRequiredService<ICurrencyFactory>();
            var regionFactory = this.serviceProviderFixture.Services.GetRequiredService<IRegionFactory>();

            var myCurrencyDotNet = new CurrencyConverterApiDotCom(currencyFactory, this.timeProvider, new CurrencyConverterApiDotCom.FreePlan(this.currencyConverterApiKey));

            var pairs = await myCurrencyDotNet.GetCurrencyPairsAsync(DateTimeOffset.Now, default);

            pairs.Count().Should().BeGreaterThan(0);
        }

        [Fact]
        public async Task GetExchangeRateAsync001()
        {
            var currencyFactory = this.serviceProviderFixture.Services.GetRequiredService<ICurrencyFactory>();
            var regionFactory = this.serviceProviderFixture.Services.GetRequiredService<IRegionFactory>();

            var myCurrencyDotNet = new CurrencyConverterApiDotCom(currencyFactory, this.timeProvider, new CurrencyConverterApiDotCom.FreePlan(this.currencyConverterApiKey));

            var amd = currencyFactory.Create("AMD");
            var usd = currencyFactory.Create("USD");
            var pair = new CurrencyPair(usd, amd);

            var rate = await myCurrencyDotNet.GetExchangeRateAsync(pair, DateTimeOffset.Now, default);

            rate.Should().BeGreaterThan(decimal.One);
        }

        [Fact]
        public async Task GetExchangeRateAsync002()
        {
            var currencyFactory = this.serviceProviderFixture.Services.GetRequiredService<ICurrencyFactory>();
            var regionFactory = this.serviceProviderFixture.Services.GetRequiredService<IRegionFactory>();

            var myCurrencyDotNet = new CurrencyConverterApiDotCom(currencyFactory, this.timeProvider, new CurrencyConverterApiDotCom.FreePlan(this.currencyConverterApiKey));

            var amd = currencyFactory.Create("AMD");
            var usd = currencyFactory.Create("USD");
            var pair = new CurrencyPair(amd, usd);

            var rate = await myCurrencyDotNet.GetExchangeRateAsync(pair, DateTimeOffset.Now, default);

            rate.Should().BeLessThan(decimal.One);
        }
    }
}
