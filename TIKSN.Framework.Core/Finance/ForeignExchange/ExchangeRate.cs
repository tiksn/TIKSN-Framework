namespace TIKSN.Finance.ForeignExchange;

public class ExchangeRate : IEquatable<ExchangeRate>
{
    public ExchangeRate(CurrencyPair pair, DateTimeOffset asOn, decimal rate)
    {
        if (rate <= decimal.Zero)
        {
            throw new ArgumentOutOfRangeException(nameof(rate), rate, "Rate must be a positive number.");
        }

        this.Pair = pair ?? throw new ArgumentNullException(nameof(pair));
        this.AsOn = asOn;
        this.Rate = rate;
    }

    public DateTimeOffset AsOn { get; }

    public CurrencyPair Pair { get; }

    public decimal Rate { get; }

    public bool Equals(ExchangeRate? other)
    {
        if (other == null)
        {
            return false;
        }

        return this.AsOn == other.AsOn
               && this.Pair == other.Pair
               && this.Rate == other.Rate;
    }

    public override bool Equals(object? obj) => this.Equals(obj as ExchangeRate);

    public override int GetHashCode() =>
        this.AsOn.GetHashCode() ^ this.Pair.GetHashCode() ^ this.Rate.GetHashCode();

    public ExchangeRate Reverse() => new(this.Pair.Reverse(), this.AsOn, decimal.One / this.Rate);
}
