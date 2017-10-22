using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace TIKSN.Finance.Tests
{
    public class CompositeCrossCurrencyConverterTests
    {
        [Fact]
        public async Task GetCurrencyPairsAsync001()
        {
            var converter = new CompositeCrossCurrencyConverter(new AverageCurrencyConversionCompositionStrategy());

            converter.Add(new FixedRateCurrencyConverter(new CurrencyPair(new CurrencyInfo("USD"), new CurrencyInfo("EUR")), 1.12m));
            converter.Add(new FixedRateCurrencyConverter(new CurrencyPair(new CurrencyInfo("USD"), new CurrencyInfo("GBP")), 1.13m));
            converter.Add(new FixedRateCurrencyConverter(new CurrencyPair(new CurrencyInfo("USD"), new CurrencyInfo("CHF")), 1.14m));

            var pairs = await converter.GetCurrencyPairsAsync(DateTimeOffset.Now);

            Assert.Equal(3, pairs.Count());
        }

        [Fact]
        public async Task GetExchangeRateAsync001()
        {
            var converter = new CompositeCrossCurrencyConverter(new AverageCurrencyConversionCompositionStrategy());

            converter.Add(new FixedRateCurrencyConverter(new CurrencyPair(new CurrencyInfo("USD"), new CurrencyInfo("EUR")), 1.12m));
            converter.Add(new FixedRateCurrencyConverter(new CurrencyPair(new CurrencyInfo("USD"), new CurrencyInfo("GBP")), 1.13m));
            converter.Add(new FixedRateCurrencyConverter(new CurrencyPair(new CurrencyInfo("USD"), new CurrencyInfo("CHF")), 1.14m));

            var rate = await converter.GetExchangeRateAsync(new CurrencyPair(new CurrencyInfo("USD"), new CurrencyInfo("EUR")), DateTimeOffset.Now);

            Assert.Equal(1.12m, rate);
        }
    }
}