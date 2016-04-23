using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TIKSN.Finance
{
	public abstract class CompositeCurrencyConverter : ICompositeCurrencyConverter
	{
		protected List<ICurrencyConverter> converters;
		protected ICurrencyConversionCompositionStrategy compositionStrategy;

		public CompositeCurrencyConverter(ICurrencyConversionCompositionStrategy compositionStrategy)
		{
			if (compositionStrategy == null)
				throw new ArgumentNullException(nameof(compositionStrategy));

			this.compositionStrategy = compositionStrategy;
			this.converters = new List<ICurrencyConverter>();
		}

		public void Add(ICurrencyConverter converter)
		{
			this.converters.Add(converter);
		}

		public abstract Task<Money> ConvertCurrencyAsync(Money baseMoney, CurrencyInfo counterCurrency, DateTimeOffset asOn);

		public abstract Task<IEnumerable<CurrencyPair>> GetCurrencyPairsAsync(DateTimeOffset asOn);

		public abstract Task<decimal> GetExchangeRateAsync(CurrencyPair pair, DateTimeOffset asOn);
	}
}