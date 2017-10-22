using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TIKSN.Finance
{
    public class CompositeCrossCurrencyConverter : CompositeCurrencyConverter
    {
        public CompositeCrossCurrencyConverter(ICurrencyConversionCompositionStrategy compositionStrategy)
            : base(compositionStrategy)
        {
        }

        public override async Task<Money> ConvertCurrencyAsync(Money baseMoney, CurrencyInfo counterCurrency, DateTimeOffset asOn)
        {
            return await compositionStrategy.ConvertCurrencyAsync(baseMoney, this.converters, counterCurrency, asOn);
        }

        public override async Task<IEnumerable<CurrencyPair>> GetCurrencyPairsAsync(DateTimeOffset asOn)
        {
            var pairs = new HashSet<CurrencyPair>();

            foreach (var converter in this.converters)
            {
                var currentPairs = await converter.GetCurrencyPairsAsync(asOn);

                foreach (var currentPair in currentPairs)
                {
                    pairs.Add(currentPair);
                }
            }

            return pairs;
        }

        public override async Task<decimal> GetExchangeRateAsync(CurrencyPair pair, DateTimeOffset asOn)
        {
            return await compositionStrategy.GetExchangeRateAsync(this.converters, pair, asOn);
        }
    }
}