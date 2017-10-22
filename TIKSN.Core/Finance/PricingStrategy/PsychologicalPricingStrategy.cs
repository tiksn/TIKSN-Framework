using System;

namespace TIKSN.Finance.PricingStrategy
{
    internal class PsychologicalPricingStrategy : IPricingStrategy
    {
        public Money EstimateMarketPrice(Money basePrice)
        {
            var estimatedPrice = EstimateMarketPrice(basePrice.Amount);

            return new Money(basePrice.Currency, estimatedPrice);
        }

        public decimal EstimateMarketPrice(decimal basePrice)
        {
            decimal sign = basePrice >= decimal.Zero ? decimal.One : decimal.MinusOne;
            decimal absoluteBasePrice = Math.Abs(basePrice);
            decimal absoluteEstimatedPrice = absoluteBasePrice; //TODO: To change

            return sign * absoluteEstimatedPrice;
        }
    }
}