using System;

namespace TIKSN.Finance.PricingStrategy
{
    public class RoundingPricingStrategy : IPricingStrategy
    {
        private readonly int fitstImportantDigitsCount;

        public RoundingPricingStrategy(int fitstImportantDigitsCount)
        {
            if (fitstImportantDigitsCount < 1)
            {
                throw new ArgumentException(nameof(fitstImportantDigitsCount));
            }

            this.fitstImportantDigitsCount = fitstImportantDigitsCount;
        }

        public Money EstimateMarketPrice(Money basePrice)
        {
            var estimatedPrice = this.EstimateMarketPrice(basePrice.Amount);

            return new Money(basePrice.Currency, estimatedPrice);
        }

        public decimal EstimateMarketPrice(decimal basePrice)
        {
            var degree = (int)Math.Log10((double)basePrice);
            var norm = (decimal)Math.Pow(10.0, degree);

            var estimated = basePrice / norm;

            estimated = Math.Round(estimated, this.fitstImportantDigitsCount - 1);

            estimated *= norm;

            return estimated;
        }
    }
}
