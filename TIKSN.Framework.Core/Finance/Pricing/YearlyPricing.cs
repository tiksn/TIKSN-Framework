using TIKSN.Time;

namespace TIKSN.Finance.Pricing;

public sealed class YearlyPricing<TYear> : IYearlyPricing<TYear>, IEquatable<YearlyPricing<TYear>>
    where TYear : IYear<TYear>
{
    public YearlyPricing(TYear year, Money price)
    {
        ArgumentNullException.ThrowIfNull(year);

        this.Year = year;
        this.Price = PricingGuard.EnsureValidPrice(price);
    }

    public Money Price { get; }

    public TYear Year { get; }

    IYear IYearlyPricing.Year => this.Year;

    public static bool operator ==(YearlyPricing<TYear>? first, YearlyPricing<TYear>? second)
        => Equals(first, second);

    public static bool operator !=(YearlyPricing<TYear>? first, YearlyPricing<TYear>? second)
        => !Equals(first, second);

    public bool Equals(YearlyPricing<TYear>? other)
    {
        if (other is null)
        {
            return false;
        }

        return EqualityComparer<TYear>.Default.Equals(this.Year, other.Year) && this.Price.Equals(other.Price);
    }

    public override bool Equals(object? obj)
        => obj is YearlyPricing<TYear> other && this.Equals(other);

    public override int GetHashCode()
        => HashCode.Combine(this.Year, this.Price);
}
