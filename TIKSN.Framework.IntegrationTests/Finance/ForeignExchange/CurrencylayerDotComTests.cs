using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using TIKSN.Finance.ForeignExchange.Cumulative;
using TIKSN.Framework.IntegrationTests;
using TIKSN.Globalization;
using TIKSN.Time;
using Xunit;

namespace TIKSN.Finance.Tests.ForeignExchange
{
    [Collection("ServiceProviderCollection")]
    public class CurrencylayerDotComTests
    {
        const string skip = "API changed, code needs to be adopted";

        private readonly string accessKey = "<put your access key here>";
        private readonly ICurrencyFactory currencyFactory;
        private readonly ITimeProvider timeProvider;
        private readonly ServiceProviderFixture serviceProviderFixture;

        public CurrencylayerDotComTests(ServiceProviderFixture serviceProviderFixture)
        {
            this.currencyFactory = serviceProviderFixture.Services.GetRequiredService<ICurrencyFactory>();
            this.timeProvider = serviceProviderFixture.Services.GetRequiredService<ITimeProvider>();
            this.serviceProviderFixture = serviceProviderFixture ?? throw new ArgumentNullException(nameof(serviceProviderFixture));
        }

        [Fact(Skip = skip)]
        public async Task GetCurrencyPairs001Async()
        {
            var exchange = new CurrencylayerDotCom(this.currencyFactory, this.timeProvider, this.accessKey);

            var pairs = await exchange.GetCurrencyPairsAsync(DateTimeOffset.Now, default).ConfigureAwait(true);

            Assert.True(pairs.Count() > 0);
        }

        [Fact(Skip = skip)]
        public async Task GetExchangeRateAsync001Async()
        {
            var exchange = new CurrencylayerDotCom(this.currencyFactory, this.timeProvider, this.accessKey);

            var pair = new CurrencyPair(new CurrencyInfo("USD"), new CurrencyInfo("UAH"));

            var rate = await exchange.GetExchangeRateAsync(pair, DateTimeOffset.Now, default).ConfigureAwait(true);

            Assert.True(rate > decimal.Zero);
        }
    }
}
