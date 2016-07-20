﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TIKSN.Finance.Helpers;

namespace TIKSN.Finance
{
	public class AverageCurrencyConversionCompositionStrategy : ICurrencyConversionCompositionStrategy
	{
		public async Task<Money> ConvertCurrencyAsync(Money baseMoney, IEnumerable<ICurrencyConverter> converters, CurrencyInfo counterCurrency, DateTimeOffset asOn)
		{
			var filteredConverters = await CurrencyHelper.FilterConverters(converters, baseMoney.Currency, counterCurrency, asOn);

			var amounts = new List<decimal>();

			foreach (var converter in filteredConverters)
			{
				var convertedMoney = await converter.ConvertCurrencyAsync(baseMoney, counterCurrency, asOn);

				if (convertedMoney.Currency != counterCurrency)
					throw new Exception("Converted into wrong currency.");

				amounts.Add(convertedMoney.Amount);
			}

			var amount = amounts.Average();

			return new Money(counterCurrency, amount);
		}

		public async Task<decimal> GetExchangeRateAsync(IEnumerable<ICurrencyConverter> converters, CurrencyPair pair, DateTimeOffset asOn)
		{
			var filteredConverters = await CurrencyHelper.FilterConverters(converters, pair, asOn);

			var rates = new List<decimal>();

			foreach (var converter in filteredConverters)
			{
				var rate = await converter.GetExchangeRateAsync(pair, asOn);

				rates.Add(rate);
			}

			return rates.Average();
		}
	}
}