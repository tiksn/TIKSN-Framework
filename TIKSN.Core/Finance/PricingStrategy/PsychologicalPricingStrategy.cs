using System;

namespace TIKSN.Finance.PricingStrategy
{
    internal class PsychologicalPricingStrategy : IPricingStrategy
    {
        public Money EstimateMarketPrice(Money basePrice)
        {
            var estimatedPrice = this.EstimateMarketPrice(basePrice.Amount);

            return new Money(basePrice.Currency, estimatedPrice);
        }

        public decimal EstimateMarketPrice(decimal basePrice)
        {
            var sign = basePrice >= decimal.Zero ? decimal.One : decimal.MinusOne;
            var absoluteBasePrice = Math.Abs(basePrice);
            var absoluteEstimatedPrice = absoluteBasePrice; //TODO: To change

            return sign * absoluteEstimatedPrice;
        }
    }
}
