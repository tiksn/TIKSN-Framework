using System;

namespace TIKSN.Finance.PricingStrategy
{
	public class RoundingPricingStrategy : IPricingStrategy
	{
		private int fitstImportantDigitsCount;

		public RoundingPricingStrategy(int fitstImportantDigitsCount)
		{
			if (fitstImportantDigitsCount < 1)
				throw new ArgumentException(nameof(fitstImportantDigitsCount));

			this.fitstImportantDigitsCount = fitstImportantDigitsCount;
		}

		public Money EstimateMarketPrice(Money basePrice)
		{
			var estimatedPrice = EstimateMarketPrice(basePrice.Amount);

			return new Money(basePrice.Currency, estimatedPrice);
		}

		public decimal EstimateMarketPrice(decimal basePrice)
		{
			int degree = (int)Math.Log10((double)basePrice);
			decimal norm = (decimal)Math.Pow(10.0, degree);

			decimal estimated = basePrice / norm;

			estimated = Math.Round(estimated, fitstImportantDigitsCount - 1);

			estimated *= norm;

			return estimated;
		}
	}
}