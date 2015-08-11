using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TIKSN.Finance
{
    public class FixedRateCurrencyConverter : ICurrencyConverter
    {
        private CurrencyPair pair;
        private decimal Rate;

        public FixedRateCurrencyConverter(CurrencyPair Pair, decimal Rate)
        {
            this.CurrencyPair = Pair;

            if (Rate > decimal.Zero)
            {
                this.Rate = Rate;
            }
            else
            {
                throw new System.ArgumentException("Rate cannot be negative or zero.", "Rate");
            }
        }

        public CurrencyPair CurrencyPair
        {
            get
            {
                return this.pair;
            }
            private set
            {
                if (object.ReferenceEquals(value, null))
                    throw new System.ArgumentNullException();

                this.pair = value;
            }
        }

        public Task<Money> ConvertCurrencyAsync(Money BaseMoney, CurrencyInfo CounterCurrency, DateTimeOffset asOn)
        {
            CurrencyPair requiredPair = new CurrencyPair(BaseMoney.Currency, CounterCurrency);

            if (this.CurrencyPair == requiredPair)
            {
                return Task.FromResult(new Money(this.CurrencyPair.CounterCurrency, BaseMoney.Amount * this.Rate));
            }
            else
            {
                throw new ArgumentException("Unsupported currency pair.");
            }
        }

        public Task<IEnumerable<CurrencyPair>> GetCurrencyPairsAsync(DateTimeOffset asOn)
        {
            IEnumerable<CurrencyPair> singleItemList = new List<CurrencyPair>() { this.pair };

            return Task.FromResult(singleItemList);
        }

        public Task<decimal> GetExchangeRateAsync(CurrencyPair Pair, DateTimeOffset asOn)
        {
            if (this.CurrencyPair == Pair)
            {
                return Task.FromResult(this.Rate);
            }
            else
            {
                throw new ArgumentException("Unsupported currency pair.");
            }
        }
    }
}