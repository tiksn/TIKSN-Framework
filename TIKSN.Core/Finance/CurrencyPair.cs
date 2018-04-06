namespace TIKSN.Finance
{
    public class CurrencyPair : System.IEquatable<CurrencyPair>
    {
        public CurrencyPair(CurrencyInfo BaseCurrency, CurrencyInfo CounterCurrency)
        {
            if (BaseCurrency == CounterCurrency)
                throw new System.ArgumentException("Base currency and counter currency cannot be the same.");

            this.BaseCurrency = BaseCurrency;
            this.CounterCurrency = CounterCurrency;
        }

        public CurrencyInfo BaseCurrency { get; private set; }

        public CurrencyInfo CounterCurrency { get; private set; }

        public static bool operator !=(CurrencyPair pair1, CurrencyPair pair2)
        {
            return !Equals(pair1, pair2);
        }

        public static bool operator ==(CurrencyPair pair1, CurrencyPair pair2)
        {
            return Equals(pair1, pair2);
        }

        public bool Equals(CurrencyPair that)
        {
            if (object.ReferenceEquals(that, null))
                return false;

            if (object.ReferenceEquals(this, that))
                return true;

            return (this.BaseCurrency == that.BaseCurrency && this.CounterCurrency == that.CounterCurrency);
        }

        public override bool Equals(object that)
        {
            if (object.ReferenceEquals(that, null))
                return false;

            if (object.ReferenceEquals(this, that))
                return true;

            CurrencyPair another = that as CurrencyPair;

            if (object.ReferenceEquals(another, null))
                return false;

            return this.Equals(another);
        }

        public override int GetHashCode()
        {
            return this.ToString().GetHashCode();
        }

        public override string ToString()
        {
            return string.Format("{0}/{1}", this.BaseCurrency.ToString(), this.CounterCurrency.ToString());
        }

        private static bool Equals(CurrencyPair pair1, CurrencyPair pair2)
        {
            if (object.ReferenceEquals(pair1, null))
            {
                return object.ReferenceEquals(pair2, null);
            }
            else
            {
                return pair1.Equals(pair2);
            }
        }
    }
}