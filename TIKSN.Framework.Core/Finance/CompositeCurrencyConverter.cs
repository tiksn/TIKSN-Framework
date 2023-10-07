using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace TIKSN.Finance
{
    public abstract class CompositeCurrencyConverter : ICompositeCurrencyConverter
    {
        protected ICurrencyConversionCompositionStrategy compositionStrategy;
        protected List<ICurrencyConverter> converters;

        protected CompositeCurrencyConverter(ICurrencyConversionCompositionStrategy compositionStrategy)
        {
            this.compositionStrategy =
                compositionStrategy ?? throw new ArgumentNullException(nameof(compositionStrategy));
            this.converters = new List<ICurrencyConverter>();
        }

        public void Add(ICurrencyConverter converter) => this.converters.Add(converter);

        public abstract Task<Money> ConvertCurrencyAsync(Money baseMoney, CurrencyInfo counterCurrency,
            DateTimeOffset asOn, CancellationToken cancellationToken);

        public abstract Task<IEnumerable<CurrencyPair>> GetCurrencyPairsAsync(DateTimeOffset asOn,
            CancellationToken cancellationToken);

        public abstract Task<decimal> GetExchangeRateAsync(CurrencyPair pair, DateTimeOffset asOn,
            CancellationToken cancellationToken);
    }
}
