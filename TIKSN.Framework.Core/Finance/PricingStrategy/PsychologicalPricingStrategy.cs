namespace TIKSN.Finance.PricingStrategy;

public class PsychologicalPricingStrategy : IPricingStrategy
{
    public Money EstimateMarketPrice(Money basePrice)
    {
        ArgumentNullException.ThrowIfNull(basePrice);

        var estimatedPrice = this.EstimateMarketPrice(basePrice.Amount);

        return new Money(basePrice.Currency, estimatedPrice);
    }

    public decimal EstimateMarketPrice(decimal basePrice)
    {
        var sign = basePrice >= decimal.Zero ? decimal.One : decimal.MinusOne;
        var absoluteEstimatedPrice = Math.Abs(basePrice); //TODO: To change

        return sign * absoluteEstimatedPrice;
    }
}
