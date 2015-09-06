using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TIKSN.Finance.ForeignExchange;

namespace TIKSN.Finance.Tests.ForeignExchange
{
	[TestClass]
	public class CurrencylayerDotComTests
	{
		private string accessKey = "<put your access key here>";

		[TestMethod]
		public async Task GetCurrencyPairs001()
		{
			var exchange = new CurrencylayerDotCom(accessKey);

			var pairs = await exchange.GetCurrencyPairsAsync(DateTimeOffset.Now);

			Assert.IsTrue(pairs.Count() > 0);
		}

		[TestMethod]
		public async Task GetExchangeRateAsync001()
		{
			var exchange = new CurrencylayerDotCom(accessKey);

			var pair = new CurrencyPair(new CurrencyInfo("USD"), new CurrencyInfo("UAH"));

			var rate = await exchange.GetExchangeRateAsync(pair, DateTimeOffset.Now);

			Assert.IsTrue(rate > decimal.Zero);
		}
	}
}
