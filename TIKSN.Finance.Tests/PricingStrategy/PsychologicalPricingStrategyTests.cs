using Microsoft.VisualStudio.TestTools.UnitTesting;
using TIKSN.Finance.PricingStrategy;
using System.Collections.Generic;

namespace TIKSN.Finance.Tests.PricingStrategy
{
	[TestClass]
	public class PsychologicalPricingStrategyTests
	{
		[TestMethod]
		public void BalkTests()
		{
			var prices = new Dictionary<decimal, decimal>();

			prices.Add(1m, 0.99m);

			var strategy = new PsychologicalPricingStrategy();

			foreach (var price in prices)
			{
				var actualEstimatedPrice = strategy.EstimateMarketPrice(price.Key);
				var expectedEstimatedPrice = price.Value;

				Assert.AreEqual(expectedEstimatedPrice, actualEstimatedPrice, "Actual: {0} Expected: {1}", actualEstimatedPrice, expectedEstimatedPrice);
			}
		}
	}
}
