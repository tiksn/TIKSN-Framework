using System.Collections.Generic;

namespace TIKSN.Finance.Cache
{
	public class MemoryCachedCurrencyConverterEntry
	{
		private MemoryCachedCurrencyConverterEntry(MemoryCachedCurrencyConverterEntryKind kind, IEnumerable<CurrencyPair> currencyPairs, decimal exchangeRate)
		{
			Kind = kind;
			CurrencyPairs = currencyPairs;
			ExchangeRate = exchangeRate;
		}

		public IEnumerable<CurrencyPair> CurrencyPairs { get; }

		public decimal ExchangeRate { get; }

		public MemoryCachedCurrencyConverterEntryKind Kind { get; }

		public static MemoryCachedCurrencyConverterEntry CreateForCurrencyPairs(IEnumerable<CurrencyPair> currencyPairs)
		{
			return new MemoryCachedCurrencyConverterEntry(MemoryCachedCurrencyConverterEntryKind.CurrencyPairs,
				currencyPairs, decimal.Zero);
		}

		public static MemoryCachedCurrencyConverterEntry CreateForExchangeRate(decimal exchangeRate)
		{
			return new MemoryCachedCurrencyConverterEntry(MemoryCachedCurrencyConverterEntryKind.CurrencyPairs,
				null, exchangeRate);
		}
	}
}
