using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;
using TIKSN.DependencyInjection.Tests;
using TIKSN.Finance.ForeignExchange;
using Xunit;
using Xunit.Abstractions;

namespace TIKSN.Finance.Tests.ForeignExchange
{
	public class CurrencylayerDotComTests
	{
		private string accessKey = "<put your access key here>";
		private readonly IServiceProvider _serviceProvider;

		public CurrencylayerDotComTests(ITestOutputHelper testOutputHelper)
		{
			_serviceProvider = new TestCompositionRootSetup(testOutputHelper).CreateServiceProvider();
		}

		//[Fact]
		public async Task GetCurrencyPairs001()
		{
			var exchange = new CurrencylayerDotCom(accessKey, _serviceProvider.GetRequiredService<ILogger<CurrencylayerDotCom>>());

			var pairs = await exchange.GetCurrencyPairsAsync(DateTimeOffset.Now);

			Assert.True(pairs.Count() > 0);
		}

		//[Fact]
		public async Task GetExchangeRateAsync001()
		{
			var exchange = new CurrencylayerDotCom(accessKey, _serviceProvider.GetRequiredService<ILogger<CurrencylayerDotCom>>());

			var pair = new CurrencyPair(new CurrencyInfo("USD"), new CurrencyInfo("UAH"));

			var rate = await exchange.GetExchangeRateAsync(pair, DateTimeOffset.Now);

			Assert.True(rate > decimal.Zero);
		}
	}
}