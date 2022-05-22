//using Microsoft.VisualStudio.TestTools.UnitTesting;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using TIKSN.Finance.PricingStrategy;

//namespace TIKSN.Finance.Tests.PricingStrategy
//{
//	[TestClass]
//	public class PsychologicalPricingStrategyTests
//	{
//		[Fact]
//		public void BalkTests()
//		{
//			var prices = new Dictionary<decimal, decimal>();

// prices.Add(1m, 0.99m); prices.Add(2m, 1.99m); prices.Add(46m, 45m); prices.Add(47m, 49m);
// prices.Add(48m, 49m); prices.Add(49m, 49m); prices.Add(50m, 49m); prices.Add(55m, 55m);
// prices.Add(69m, 69m); prices.Add(8m, 7.99m); prices.Add(51m, 49m); prices.Add(83m, 85m);
// prices.Add(18m, 19m); prices.Add(364m, 369m); prices.Add(794m, 799m); prices.Add(191m, 199m);
// prices.Add(149m, 149m); prices.Add(761m, 765m); prices.Add(154m, 155m); prices.Add(9444m, 9449m);
// prices.Add(658m, 659m); prices.Add(1776m, 1779m); prices.Add(9267m, 9299m); prices.Add(3176m,
// 3199m); prices.Add(50771m, 50799m); prices.Add(68423m, 68999m); prices.Add(97360m, 97999m);
// prices.Add(762783m, 769999m); prices.Add(6283m, 6299m); prices.Add(870387m, 870000m);
// prices.Add(526213m, 525555m); prices.Add(418130m, 419999m);

// var strategy = new PsychologicalPricingStrategy();

// foreach (var price in prices) { var actualEstimatedPrice =
// strategy.EstimateMarketPrice(price.Key); var expectedEstimatedPrice = price.Value;

// Assert.Equal(expectedEstimatedPrice, actualEstimatedPrice, "Actual: {0} Expected: {1}",
// actualEstimatedPrice, expectedEstimatedPrice); } }

// [Fact] public void EstimatedPriceIsPrimarelyBeneficial() { var ratios = new List<decimal>();

// var strategy = new PsychologicalPricingStrategy();

// var RNG = new Random();

// for (int i = 0; i < 100; i++) { var initialPrice = (decimal)RNG.NextDouble(); var estimatedPrice = strategy.EstimateMarketPrice(initialPrice);

// ratios.Add(estimatedPrice / initialPrice); }

// for (int i = 0; i < 100; i++) { var initialPrice = (decimal)RNG.NextDouble() * 10m; var
// estimatedPrice = strategy.EstimateMarketPrice(initialPrice);

// ratios.Add(estimatedPrice / initialPrice); }

// for (int i = 0; i < 100; i++) { var initialPrice = (decimal)RNG.NextDouble() * 100m; var
// estimatedPrice = strategy.EstimateMarketPrice(initialPrice);

// ratios.Add(estimatedPrice / initialPrice); }

// for (int i = 0; i < 100; i++) { var initialPrice = (decimal)RNG.Next(0, 10); var estimatedPrice = strategy.EstimateMarketPrice(initialPrice);

// ratios.Add(estimatedPrice / initialPrice); }

// for (int i = 0; i < 100; i++) { var initialPrice = (decimal)RNG.Next(10, 100); var estimatedPrice
// = strategy.EstimateMarketPrice(initialPrice);

// ratios.Add(estimatedPrice / initialPrice); }

// for (int i = 0; i < 100; i++) { var initialPrice = (decimal)RNG.Next(100, 10000); var
// estimatedPrice = strategy.EstimateMarketPrice(initialPrice);

// ratios.Add(estimatedPrice / initialPrice); }

// for (int i = 0; i < 100; i++) { var initialPrice = (decimal)RNG.Next(); var estimatedPrice = strategy.EstimateMarketPrice(initialPrice);

// ratios.Add(estimatedPrice / initialPrice); }

// var averageRatio = ratios.Average(); var averagePercentage = averageRatio * 100m;

//			Assert.True(averagePercentage >= 100);
//			Assert.True(averagePercentage <= 102);
//		}
//	}
//}
