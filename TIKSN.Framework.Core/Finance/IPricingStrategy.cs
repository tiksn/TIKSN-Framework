namespace TIKSN.Finance;

public interface IPricingStrategy
{
    decimal EstimateMarketPrice(decimal basePrice);

    Money EstimateMarketPrice(Money basePrice);
}
