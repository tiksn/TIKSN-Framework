using System;

namespace TIKSN.Finance.ForeignExchange
{
    public class ExchangeRate : IEquatable<ExchangeRate>
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

        public DateTimeOffset AsOn { get; }

        public CurrencyPair Pair { get; }

        public decimal Rate { get; }

        public bool Equals(ExchangeRate other)
        {
            if (other == null)
                return false;

            return AsOn == other.AsOn
                && Pair == other.Pair
                && Rate == other.Rate;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as ExchangeRate);
        }

        public ExchangeRate Reverse()
        {
            return new ExchangeRate(Pair.Reverse(), AsOn, decimal.One / Rate);
        }

        public override int GetHashCode()
        {
            return AsOn.GetHashCode() ^ Pair.GetHashCode() ^ Rate.GetHashCode();
        }
    }
}