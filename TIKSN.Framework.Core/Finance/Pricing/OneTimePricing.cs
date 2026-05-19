namespace TIKSN.Finance.Pricing;

public sealed class OneTimePricing : IPricing, IEquatable<OneTimePricing>
{
    public OneTimePricing(Money price)
        => this.Price = PricingGuard.EnsureValidPrice(price);

    public Money Price { get; }

    public static bool operator ==(OneTimePricing? first, OneTimePricing? second)
        => Equals(first, second);

    public static bool operator !=(OneTimePricing? first, OneTimePricing? second)
        => !Equals(first, second);

    public bool Equals(OneTimePricing? other)
    {
        if (other is null)
        {
            return false;
        }

        return this.Price.Equals(other.Price);
    }

    public override bool Equals(object? obj)
        => obj is OneTimePricing other && this.Equals(other);

    public override int GetHashCode()
        => this.Price.GetHashCode();
}
