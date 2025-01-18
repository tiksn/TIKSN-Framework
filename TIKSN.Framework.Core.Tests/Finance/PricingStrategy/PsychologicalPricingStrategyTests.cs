using System;
using System.Collections.Generic;
using System.Linq;
using TIKSN.Finance;
using TIKSN.Finance.PricingStrategy;
using Xunit;

namespace TIKSN.Tests.Finance.PricingStrategy;

public class PsychologicalPricingStrategyTests
{
    [Theory]
    [InlineData(-1, -0.99)]
    [InlineData(0, 0)]
    [InlineData(0.1, 0.1)]
    [InlineData(1, 0.99)]
    [InlineData(2, 1.99)]
    [InlineData(8, 7.99)]
    [InlineData(46, 45.99)]
    [InlineData(47, 46.99)]
    [InlineData(48, 47.99)]
    [InlineData(49, 48.99)]
    [InlineData(50, 49.99)]
    [InlineData(55, 54.99)]
    [InlineData(69, 68.99)]
    [InlineData(51, 50.99)]
    [InlineData(83, 82.99)]
    [InlineData(18, 17.99)]
    [InlineData(364, 369)]
    [InlineData(794, 799)]
    [InlineData(191, 199)]
    [InlineData(149, 149)]
    [InlineData(761, 769)]
    [InlineData(154, 159)]
    [InlineData(9444, 9499)]
    [InlineData(658, 659)]
    [InlineData(1776, 1799)]
    [InlineData(9267, 9299)]
    [InlineData(3176, 3199)]
    [InlineData(50771, 50999)]
    [InlineData(68423, 68999)]
    [InlineData(97360, 97999)]
    [InlineData(762783, 769999)]
    [InlineData(6283, 6299)]
    [InlineData(870387, 879999)]
    [InlineData(526213, 529999)]
    [InlineData(418130, 419999)]
    public void BalkTests(double originalPrice, double expectedEstimatedPrice)
    {
        var currency = new CurrencyInfo("USD");
        var strategy = new PsychologicalPricingStrategy();

        var actualEstimatedPrice = strategy.EstimateMarketPrice(new Money(currency, (decimal)originalPrice));

        _ = actualEstimatedPrice.Currency.Should().Be(currency);
        _ = actualEstimatedPrice.Amount.Should().Be((decimal)expectedEstimatedPrice);
    }

    [Fact]
    public void EstimatedPriceIsPrimarilyBeneficial()
    {
        var currency = new CurrencyInfo("USD");

        var ratios = new List<decimal>();

        var strategy = new PsychologicalPricingStrategy();

        var random = new Random();

        for (var i = 0; i < 100; i++)
        {
            var initialPrice = (decimal)random.NextDouble();
            var estimatedPrice = strategy.EstimateMarketPrice(new Money(currency, initialPrice));

            AddRatio(ratios, initialPrice, estimatedPrice.Amount);
        }

        for (var i = 0; i < 100; i++)
        {
            var initialPrice = (decimal)random.NextDouble() * 10m;
            var estimatedPrice = strategy.EstimateMarketPrice(new Money(currency, initialPrice));

            AddRatio(ratios, initialPrice, estimatedPrice.Amount);
        }

        for (var i = 0; i < 100; i++)
        {
            var initialPrice = (decimal)(random.NextDouble() * 100);
            var estimatedPrice = strategy.EstimateMarketPrice(new Money(currency, initialPrice));

            AddRatio(ratios, initialPrice, estimatedPrice.Amount);
        }

        for (var i = 0; i < 100; i++)
        {
            var initialPrice = random.Next(0, 10);
            var estimatedPrice = strategy.EstimateMarketPrice(new Money(currency, initialPrice));

            AddRatio(ratios, initialPrice, estimatedPrice.Amount);
        }

        for (var i = 0; i < 100; i++)
        {
            var initialPrice = random.Next(10, 100);
            var estimatedPrice = strategy.EstimateMarketPrice(new Money(currency, initialPrice));

            AddRatio(ratios, initialPrice, estimatedPrice.Amount);
        }

        for (var i = 0; i < 100; i++)
        {
            var initialPrice = random.Next(100, 10000);
            var estimatedPrice = strategy.EstimateMarketPrice(new Money(currency, initialPrice));

            AddRatio(ratios, initialPrice, estimatedPrice.Amount);
        }

        for (var i = 0; i < 100; i++)
        {
            var initialPrice = random.Next();
            var estimatedPrice = strategy.EstimateMarketPrice(new Money(currency, initialPrice));

            AddRatio(ratios, initialPrice, estimatedPrice.Amount);
        }

        var averageRatio = ratios.Average();
        var averagePercentage = averageRatio * 100m;

        _ = averagePercentage.Should().BeGreaterThanOrEqualTo(100);
        _ = averagePercentage.Should().BeLessThanOrEqualTo(102);

        static void AddRatio(List<decimal> ratios, decimal initialPrice, decimal estimatedPrice)
        {
            if (initialPrice == decimal.Zero && estimatedPrice == decimal.Zero)
            {
                ratios.Add(decimal.One);
            }
            else
            {
                ratios.Add(estimatedPrice / initialPrice);
            }
        }
    }
}
