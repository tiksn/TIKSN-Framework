namespace TIKSN.Finance.PricingStrategy;

public interface IPricingStrategy
{
    public Money EstimateMarketPrice(Money basePrice);
}
