using TIKSN.Time;

namespace TIKSN.Finance.Pricing;

public sealed class DailyPricing<TDay> : IDailyPricing<TDay>, IEquatable<DailyPricing<TDay>>
    where TDay : IDay<TDay>
{
    public DailyPricing(TDay day, Money price)
    {
        ArgumentNullException.ThrowIfNull(day);

        this.Day = day;
        this.Price = PricingGuard.EnsureValidPrice(price);
    }

    public TDay Day { get; }

    public Money Price { get; }

    IDay IDailyPricing.Day => this.Day;

    public static bool operator ==(DailyPricing<TDay>? first, DailyPricing<TDay>? second)
        => Equals(first, second);

    public static bool operator !=(DailyPricing<TDay>? first, DailyPricing<TDay>? second)
        => !Equals(first, second);

    public bool Equals(DailyPricing<TDay>? other)
    {
        if (other is null)
        {
            return false;
        }

        return EqualityComparer<TDay>.Default.Equals(this.Day, other.Day) && this.Price.Equals(other.Price);
    }

    public override bool Equals(object? obj)
        => obj is DailyPricing<TDay> other && this.Equals(other);

    public override int GetHashCode()
        => HashCode.Combine(this.Day, this.Price);
}
