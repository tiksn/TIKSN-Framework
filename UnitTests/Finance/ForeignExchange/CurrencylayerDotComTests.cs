using System;
using System.Linq;
using System.Threading.Tasks;
using TIKSN.Finance.ForeignExchange;
using Xunit;

namespace TIKSN.Finance.Tests.ForeignExchange
{
    public class CurrencylayerDotComTests
	{
		private string accessKey = "<put your access key here>";

        //[Fact]
        public async Task GetCurrencyPairs001()
		{
			var exchange = new CurrencylayerDotCom(accessKey);

			var pairs = await exchange.GetCurrencyPairsAsync(DateTimeOffset.Now);

			Assert.True(pairs.Count() > 0);
		}

		//[Fact]
		public async Task GetExchangeRateAsync001()
		{
			var exchange = new CurrencylayerDotCom(accessKey);

			var pair = new CurrencyPair(new CurrencyInfo("USD"), new CurrencyInfo("UAH"));

			var rate = await exchange.GetExchangeRateAsync(pair, DateTimeOffset.Now);

			Assert.True(rate > decimal.Zero);
		}
	}
}