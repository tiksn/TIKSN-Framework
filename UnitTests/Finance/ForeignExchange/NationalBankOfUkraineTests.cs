using System;
using System.Linq;
using System.Threading.Tasks;
using TIKSN.Finance.ForeignExchange;
using Xunit;

namespace TIKSN.Finance.Tests.ForeignExchange
{
	public class NationalBankOfUkraineTests
	{
		[Fact]
		public async Task ConvertCurrencyAsync001()
		{
			var date = new DateTimeOffset(2016, 05, 06, 0, 0, 0, TimeSpan.Zero);
			var nbu = new NationalBankOfUkraine();
			var pairs = await nbu.GetCurrencyPairsAsync(date);

			foreach (var pair in pairs)
			{
				var baseMoney = new Money(pair.BaseCurrency, 100);
				var convertedMoney = await nbu.ConvertCurrencyAsync(baseMoney, pair.CounterCurrency, date);

				Assert.Equal(pair.CounterCurrency, convertedMoney.Currency);
				Assert.True(convertedMoney.Amount > decimal.Zero);
			}
		}

		[Fact]
		public async Task GetCurrencyPairsAsync001()
		{
			var nbu = new NationalBankOfUkraine();

			var pairs = await nbu.GetCurrencyPairsAsync(new DateTimeOffset(2016, 05, 06, 0, 0, 0, TimeSpan.Zero));

			Assert.True(pairs.Any());
		}

		[Fact]
		public async Task GetExchangeRateAsync001()
		{
			var date = new DateTimeOffset(2016, 05, 06, 0, 0, 0, TimeSpan.Zero);
			var nbu = new NationalBankOfUkraine();
			var pairs = await nbu.GetCurrencyPairsAsync(date);

			foreach (var pair in pairs)
			{
				var rate = await nbu.GetExchangeRateAsync(pair, date);

				Assert.True(rate > decimal.Zero);
			}
		}
	}
}