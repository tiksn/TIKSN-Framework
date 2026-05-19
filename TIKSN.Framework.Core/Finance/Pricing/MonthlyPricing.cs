using TIKSN.Time;

namespace TIKSN.Finance.Pricing;

public sealed class MonthlyPricing<TMonth> : IMonthlyPricing<TMonth>, IEquatable<MonthlyPricing<TMonth>>
    where TMonth : IMonth<TMonth>
{
    public MonthlyPricing(TMonth month, Money price)
    {
        ArgumentNullException.ThrowIfNull(month);

        this.Month = month;
        this.Price = PricingGuard.EnsureValidPrice(price);
    }

    public TMonth Month { get; }

    public Money Price { get; }

    IMonth IMonthlyPricing.Month => this.Month;

    public static bool operator ==(MonthlyPricing<TMonth>? first, MonthlyPricing<TMonth>? second)
        => Equals(first, second);

    public static bool operator !=(MonthlyPricing<TMonth>? first, MonthlyPricing<TMonth>? second)
        => !Equals(first, second);

    public bool Equals(MonthlyPricing<TMonth>? other)
    {
        if (other is null)
        {
            return false;
        }

        return EqualityComparer<TMonth>.Default.Equals(this.Month, other.Month) && this.Price.Equals(other.Price);
    }

    public override bool Equals(object? obj)
        => obj is MonthlyPricing<TMonth> other && this.Equals(other);

    public override int GetHashCode()
        => HashCode.Combine(this.Month, this.Price);
}
