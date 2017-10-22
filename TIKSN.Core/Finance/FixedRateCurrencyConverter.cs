using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TIKSN.Finance
{
    public class FixedRateCurrencyConverter : ICurrencyConverter
    {
        private CurrencyPair currencyPair;
        private decimal rate;

        public FixedRateCurrencyConverter(CurrencyPair pair, decimal rate)
        {
            this.CurrencyPair = pair;

            if (rate > decimal.Zero)
            {
                this.rate = rate;
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
                return this.currencyPair;
            }
            private set
            {
                if (object.ReferenceEquals(value, null))
                    throw new ArgumentNullException();

                this.currencyPair = value;
            }
        }

        public Task<Money> ConvertCurrencyAsync(Money baseMoney, CurrencyInfo counterCurrency, DateTimeOffset asOn)
        {
            CurrencyPair requiredPair = new CurrencyPair(baseMoney.Currency, counterCurrency);

            if (this.CurrencyPair == requiredPair)
            {
                return Task.FromResult(new Money(this.CurrencyPair.CounterCurrency, baseMoney.Amount * this.rate));
            }
            else
            {
                throw new ArgumentException("Unsupported currency pair.");
            }
        }

        public Task<IEnumerable<CurrencyPair>> GetCurrencyPairsAsync(DateTimeOffset asOn)
        {
            IEnumerable<CurrencyPair> singleItemList = new List<CurrencyPair>() { this.currencyPair };

            return Task.FromResult(singleItemList);
        }

        public Task<decimal> GetExchangeRateAsync(CurrencyPair pair, DateTimeOffset asOn)
        {
            if (this.CurrencyPair == pair)
            {
                return Task.FromResult(this.rate);
            }
            else
            {
                throw new ArgumentException("Unsupported currency pair.");
            }
        }
    }
}