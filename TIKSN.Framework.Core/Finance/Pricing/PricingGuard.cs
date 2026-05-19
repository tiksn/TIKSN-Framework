namespace TIKSN.Finance.Pricing;

internal static class PricingGuard
{
    public static Money EnsureValidPrice(Money price)
    {
        ArgumentNullException.ThrowIfNull(price);
        ArgumentOutOfRangeException.ThrowIfNegative(price.Amount);

        return price;
    }
}
