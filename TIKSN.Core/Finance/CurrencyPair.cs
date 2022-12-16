using System;

namespace TIKSN.Finance
{
    public class CurrencyPair : IEquatable<CurrencyPair>
    {
        public CurrencyPair(CurrencyInfo BaseCurrency, CurrencyInfo CounterCurrency)
        {
            if (BaseCurrency == CounterCurrency)
            {
                throw new ArgumentException("Base currency and counter currency cannot be the same.");
            }

            this.BaseCurrency = BaseCurrency;
            this.CounterCurrency = CounterCurrency;
        }

        public CurrencyInfo BaseCurrency { get; }

        public CurrencyInfo CounterCurrency { get; }

        public bool Equals(CurrencyPair other)
        {
            if (other is null)
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return this.BaseCurrency == other.BaseCurrency && this.CounterCurrency == other.CounterCurrency;
        }

        public static bool operator !=(CurrencyPair pair1, CurrencyPair pair2) => !Equals(pair1, pair2);

        public static bool operator ==(CurrencyPair pair1, CurrencyPair pair2) => Equals(pair1, pair2);

        public override bool Equals(object obj)
        {
            if (obj is null)
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }


            if (obj is not CurrencyPair another)
            {
                return false;
            }

            return this.Equals(another);
        }

        public override int GetHashCode() => this.ToString().GetHashCode();

        public CurrencyPair Reverse() => new(this.CounterCurrency, this.BaseCurrency);

        public override string ToString() => string.Format("{0}/{1}", this.BaseCurrency, this.CounterCurrency);

        private static bool Equals(CurrencyPair pair1, CurrencyPair pair2)
        {
            if (pair1 is null)
            {
                return pair2 is null;
            }

            return pair1.Equals(pair2);
        }
    }
}
