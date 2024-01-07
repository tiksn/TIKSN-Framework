namespace TIKSN.Finance.PricingStrategy;

public interface IPricingStrategy
{
    Money EstimateMarketPrice(Money basePrice);
}
