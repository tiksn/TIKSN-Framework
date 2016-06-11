using System.Collections.Generic;
using TIKSN.Finance.PricingStrategy;
using Xunit;

namespace TIKSN.Finance.Tests.PricingStrategy
{
	public class RoundingPricingStrategyTests
	{
		[Fact]
		public void EstimateMarketPrice001()
		{
			var strategy = new RoundingPricingStrategy(1);

			var prices = new Dictionary<decimal, decimal>();

			prices.Add(1m, 1m);
			prices.Add(2m, 2m);
			prices.Add(46m, 50m);
			prices.Add(47m, 50m);
			prices.Add(48m, 50m);
			prices.Add(49m, 50m);
			prices.Add(50m, 50m);
			prices.Add(55m, 60m);
			prices.Add(69m, 70m);
			prices.Add(8m, 8m);
			prices.Add(51m, 50m);
			prices.Add(83m, 80m);
			prices.Add(18m, 20m);
			prices.Add(364m, 400m);
			prices.Add(794m, 800m);
			prices.Add(191m, 200m);
			prices.Add(149m, 100m);
			prices.Add(761m, 800m);
			prices.Add(154m, 200m);
			prices.Add(6283m, 6000m);
			prices.Add(870387m, 900000m);
			prices.Add(526213m, 500000m);

			foreach (var price in prices)
			{
				var actualEstimatedPrice = strategy.EstimateMarketPrice(price.Key);
				var expectedEstimatedPrice = price.Value;

				Assert.Equal(expectedEstimatedPrice, actualEstimatedPrice, "Actual: {0} Expected: {1}", actualEstimatedPrice, expectedEstimatedPrice);
			}
		}

		[Fact]
		public void EstimateMarketPrice002()
		{
			var strategy = new RoundingPricingStrategy(2);

			var prices = new Dictionary<decimal, decimal>();

			prices.Add(1m, 1m);
			prices.Add(2m, 2m);
			prices.Add(49m, 49m);
			prices.Add(50m, 50m);
			prices.Add(55m, 55m);
			prices.Add(364m, 360m);
			prices.Add(794m, 790m);
			prices.Add(191m, 190m);
			prices.Add(149m, 150m);
			prices.Add(6283m, 6300m);

			foreach (var price in prices)
			{
				var actualEstimatedPrice = strategy.EstimateMarketPrice(price.Key);
				var expectedEstimatedPrice = price.Value;

				Assert.Equal(expectedEstimatedPrice, actualEstimatedPrice, "Actual: {0} Expected: {1}", actualEstimatedPrice, expectedEstimatedPrice);
			}
		}
	}
}