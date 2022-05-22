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

        public bool Equals(CurrencyPair that)
        {
            if (that is null)
            {
                return false;
            }

            if (ReferenceEquals(this, that))
            {
                return true;
            }

            return this.BaseCurrency == that.BaseCurrency && this.CounterCurrency == that.CounterCurrency;
        }

        public static bool operator !=(CurrencyPair pair1, CurrencyPair pair2) => !Equals(pair1, pair2);

        public static bool operator ==(CurrencyPair pair1, CurrencyPair pair2) => Equals(pair1, pair2);

        public override bool Equals(object that)
        {
            if (that is null)
            {
                return false;
            }

            if (ReferenceEquals(this, that))
            {
                return true;
            }


            if (that is not CurrencyPair another)
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
