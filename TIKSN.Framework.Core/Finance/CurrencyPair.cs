namespace TIKSN.Finance;

public sealed class CurrencyPair : IEquatable<CurrencyPair>
{
    public CurrencyPair(CurrencyInfo baseCurrency, CurrencyInfo counterCurrency)
    {
        if (baseCurrency == counterCurrency)
        {
            throw new ArgumentException("Base currency and counter currency cannot be the same.", nameof(counterCurrency));
        }

        this.BaseCurrency = baseCurrency;
        this.CounterCurrency = counterCurrency;
    }

    public CurrencyInfo BaseCurrency { get; }

    public CurrencyInfo CounterCurrency { get; }

    public static bool operator !=(CurrencyPair pair1, CurrencyPair pair2) => !Equals(pair1, pair2);

    public static bool operator ==(CurrencyPair pair1, CurrencyPair pair2) => Equals(pair1, pair2);

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

    public override int GetHashCode() => StringComparer.Ordinal.GetHashCode(this.ToString());

    public CurrencyPair Reverse() => new(this.CounterCurrency, this.BaseCurrency);

    public override string ToString() => $"{this.BaseCurrency}/{this.CounterCurrency}";

    private static bool Equals(CurrencyPair pair1, CurrencyPair pair2)
    {
        if (pair1 is null)
        {
            return pair2 is null;
        }

        return pair1.Equals(pair2);
    }
}
