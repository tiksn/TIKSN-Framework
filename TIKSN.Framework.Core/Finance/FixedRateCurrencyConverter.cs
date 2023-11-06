using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace TIKSN.Finance
{
    public class FixedRateCurrencyConverter : ICurrencyConverter
    {
        private readonly decimal rate;

        public FixedRateCurrencyConverter(CurrencyPair pair, decimal rate)
        {
            this.CurrencyPair = pair ?? throw new ArgumentNullException(nameof(pair));

            if (rate > decimal.Zero)
            {
                this.rate = rate;
            }
            else
            {
                throw new ArgumentException("Rate cannot be negative or zero.", "Rate");
            }
        }

        public CurrencyPair CurrencyPair { get; }

        public Task<Money> ConvertCurrencyAsync(Money baseMoney, CurrencyInfo counterCurrency, DateTimeOffset asOn,
            CancellationToken cancellationToken)
        {
            var requiredPair = new CurrencyPair(baseMoney.Currency, counterCurrency);

            if (this.CurrencyPair == requiredPair)
            {
                return Task.FromResult(new Money(this.CurrencyPair.CounterCurrency, baseMoney.Amount * this.rate));
            }

            throw new ArgumentException("Unsupported currency pair.");
        }

        public Task<IEnumerable<CurrencyPair>> GetCurrencyPairsAsync(DateTimeOffset asOn,
            CancellationToken cancellationToken)
        {
            IEnumerable<CurrencyPair> singleItemList = new List<CurrencyPair> { this.CurrencyPair };

            return Task.FromResult(singleItemList);
        }

        public Task<decimal> GetExchangeRateAsync(CurrencyPair pair, DateTimeOffset asOn,
            CancellationToken cancellationToken)
        {
            if (this.CurrencyPair == pair)
            {
                return Task.FromResult(this.rate);
            }

            throw new ArgumentException("Unsupported currency pair.");
        }
    }
}
