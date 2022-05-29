using System;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TIKSN.Finance.ForeignExchange.Cumulative;
using TIKSN.Globalization;
using TIKSN.IntegrationTests;
using TIKSN.Time;
using Xunit;

namespace TIKSN.Finance.ForeignExchange.IntegrationTests
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

            _ = this.serviceProviderFixture.Services.GetRequiredService<IRegionFactory>();

            var myCurrencyDotNet = new CurrencyConverterApiDotCom(currencyFactory, this.timeProvider, new CurrencyConverterApiDotCom.FreePlan(this.currencyConverterApiKey));

            var pairs = await myCurrencyDotNet.GetCurrencyPairsAsync(DateTimeOffset.Now, default).ConfigureAwait(true);

            _ = pairs.Count().Should().BeGreaterThan(0);
        }

        [Fact]
        public async Task GetExchangeRateAsync001Async()
        {
            var currencyFactory = this.serviceProviderFixture.Services.GetRequiredService<ICurrencyFactory>();

            _ = this.serviceProviderFixture.Services.GetRequiredService<IRegionFactory>();

            var myCurrencyDotNet = new CurrencyConverterApiDotCom(currencyFactory, this.timeProvider, new CurrencyConverterApiDotCom.FreePlan(this.currencyConverterApiKey));

            var amd = currencyFactory.Create("AMD");
            var usd = currencyFactory.Create("USD");
            var pair = new CurrencyPair(usd, amd);

            var rate = await myCurrencyDotNet.GetExchangeRateAsync(pair, DateTimeOffset.Now, default).ConfigureAwait(true);

            _ = rate.Should().BeGreaterThan(decimal.One);
        }

        [Fact]
        public async Task GetExchangeRateAsync002Async()
        {
            var currencyFactory = this.serviceProviderFixture.Services.GetRequiredService<ICurrencyFactory>();

            _ = this.serviceProviderFixture.Services.GetRequiredService<IRegionFactory>();

            var myCurrencyDotNet = new CurrencyConverterApiDotCom(currencyFactory, this.timeProvider, new CurrencyConverterApiDotCom.FreePlan(this.currencyConverterApiKey));

            var amd = currencyFactory.Create("AMD");
            var usd = currencyFactory.Create("USD");
            var pair = new CurrencyPair(amd, usd);

            var rate = await myCurrencyDotNet.GetExchangeRateAsync(pair, DateTimeOffset.Now, default).ConfigureAwait(true);

            _ = rate.Should().BeLessThan(decimal.One);
        }
    }
}
