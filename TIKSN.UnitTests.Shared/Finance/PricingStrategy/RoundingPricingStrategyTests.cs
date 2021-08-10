using System.Collections.Generic;
using Xunit;

namespace TIKSN.Finance.PricingStrategy.Tests
{
    public class RoundingPricingStrategyTests
    {
        [Fact]
        public void EstimateMarketPrice001()
        {
            var strategy = new RoundingPricingStrategy(1);

            var prices = new Dictionary<decimal, decimal>
            {
                { 1m, 1m },
                { 2m, 2m },
                { 46m, 50m },
                { 47m, 50m },
                { 48m, 50m },
                { 49m, 50m },
                { 50m, 50m },
                { 55m, 60m },
                { 69m, 70m },
                { 8m, 8m },
                { 51m, 50m },
                { 83m, 80m },
                { 18m, 20m },
                { 364m, 400m },
                { 794m, 800m },
                { 191m, 200m },
                { 149m, 100m },
                { 761m, 800m },
                { 154m, 200m },
                { 6283m, 6000m },
                { 870387m, 900000m },
                { 526213m, 500000m }
            };

            foreach (var price in prices)
            {
                var actualEstimatedPrice = strategy.EstimateMarketPrice(price.Key);
                var expectedEstimatedPrice = price.Value;

                Assert.Equal(expectedEstimatedPrice, actualEstimatedPrice);
            }
        }

        [Fact]
        public void EstimateMarketPrice002()
        {
            var strategy = new RoundingPricingStrategy(2);

            var prices = new Dictionary<decimal, decimal>
            {
                { 1m, 1m },
                { 2m, 2m },
                { 49m, 49m },
                { 50m, 50m },
                { 55m, 55m },
                { 364m, 360m },
                { 794m, 790m },
                { 191m, 190m },
                { 149m, 150m },
                { 6283m, 6300m }
            };

            foreach (var price in prices)
            {
                var actualEstimatedPrice = strategy.EstimateMarketPrice(price.Key);
                var expectedEstimatedPrice = price.Value;

                Assert.Equal(expectedEstimatedPrice, actualEstimatedPrice);
            }
        }
    }
}
