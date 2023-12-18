namespace TIKSN.Finance.PricingStrategy;

public class RoundingPricingStrategy : IPricingStrategy
{
    private readonly int firstImportantDigitsCount;

    public RoundingPricingStrategy(int firstImportantDigitsCount)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(firstImportantDigitsCount, 1);

        this.firstImportantDigitsCount = firstImportantDigitsCount;
    }

    public Money EstimateMarketPrice(Money basePrice)
    {
        ArgumentNullException.ThrowIfNull(basePrice);

        var estimatedPrice = this.EstimateMarketPrice(basePrice.Amount);

        return new Money(basePrice.Currency, estimatedPrice);
    }

    public decimal EstimateMarketPrice(decimal basePrice)
    {
        var degree = (int)Math.Log10((double)basePrice);
        var norm = (decimal)Math.Pow(10.0, degree);

        var estimated = basePrice / norm;

        estimated = Math.Round(estimated, this.firstImportantDigitsCount - 1);

        estimated *= norm;

        return estimated;
    }
}
