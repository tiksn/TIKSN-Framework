using System;

namespace TIKSN.Finance.ForeignExchange
{
    public class ExchangeRate
    {
        public ExchangeRate(CurrencyPair pair, DateTimeOffset asOn, decimal rate)
        {
            if (pair == null)
                throw new ArgumentNullException(nameof(pair));

            if (rate <= decimal.Zero) throw new ArgumentOutOfRangeException(nameof(rate), rate, "Rate must be a positive number.");

            Pair = pair;
            AsOn = asOn;
            Rate = rate;
        }

        public CurrencyPair Pair { get; }
        public decimal Rate { get; }
        public DateTimeOffset AsOn { get; }
    }
}