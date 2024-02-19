namespace TIKSN.Finance.PricingStrategy;

public class PsychologicalPricingStrategy : IPricingStrategy
{
    public Money EstimateMarketPrice(Money basePrice)
    {
        ArgumentNullException.ThrowIfNull(basePrice);

        var estimatedPrice = EstimateMarketPrice(basePrice.Amount);

        return new Money(basePrice.Currency, estimatedPrice);
    }

    private static decimal EstimateMarketPrice(decimal basePrice)
    {
        if (basePrice < decimal.Zero)
        {
            return EstimateMarketPrice(basePrice * decimal.MinusOne) * decimal.MinusOne;
        }

        if (basePrice < decimal.One)
        {
            return basePrice;
        }

        if (basePrice < 100m)
        {
            return Math.Round(basePrice) - 0.01m;
        }

        if (basePrice < 1000m)
        {
            return (Math.Floor(basePrice / 10m) * 10m) + 9m;
        }

        var degree = (int)Math.Log10((double)basePrice);
        var norm = (decimal)Math.Pow(10.0, degree - 1);

        var estimated = basePrice / norm;

        estimated = Math.Floor(estimated);
        estimated = (estimated * norm) + norm - 1;

        return estimated;
    }
}
