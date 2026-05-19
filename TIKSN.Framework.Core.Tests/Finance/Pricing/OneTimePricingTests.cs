using System;
using Shouldly;
using TIKSN.Finance.Pricing;
using Xunit;

namespace TIKSN.Tests.Finance.Pricing;

public class OneTimePricingTests
{
    [Fact]
    public void GivenDifferentValues_WhenCompared_ThenShouldNotBeEqual()
    {
        // Arrange

        var first = new OneTimePricing(PricingTestHelper.CreateMoney(10m));
        var second = new OneTimePricing(PricingTestHelper.CreateMoney(11m));

        // Act and Assert

        first.ShouldNotBe(second);
        (first == second).ShouldBeFalse();
        (first != second).ShouldBeTrue();
    }

    [Fact]
    public void GivenNegativePrice_WhenCreated_ThenShouldThrow()
    {
        // Arrange

        var price = PricingTestHelper.CreateMoney(-1m);

        // Act

        Action action = () => _ = new OneTimePricing(price);

        // Assert

        _ = action.ShouldThrow<ArgumentOutOfRangeException>();
    }

    [Fact]
    public void GivenNullPrice_WhenCreated_ThenShouldThrow()
    {
        // Act

        Action action = () => _ = new OneTimePricing(null);

        // Assert

        _ = action.ShouldThrow<ArgumentNullException>();
    }

    [Fact]
    public void GivenOneTimePricing_WhenCreated_ThenPriceShouldBeExposed()
    {
        // Arrange

        var price = PricingTestHelper.CreateMoney(100m);

        // Act

        var pricing = new OneTimePricing(price);

        // Assert

        pricing.Price.ShouldBe(price);
        pricing.Price.ShouldBe(price);
    }

    [Fact]
    public void GivenSameValues_WhenCompared_ThenShouldBeEqual()
    {
        // Arrange

        var first = new OneTimePricing(PricingTestHelper.CreateMoney(10m));
        var second = new OneTimePricing(PricingTestHelper.CreateMoney(10m));

        // Act and Assert

        first.ShouldBe(second);
        (first == second).ShouldBeTrue();
        (first != second).ShouldBeFalse();
    }

    [Fact]
    public void GivenZeroPrice_WhenCreated_ThenShouldBeAccepted()
    {
        // Arrange

        var price = PricingTestHelper.CreateMoney(decimal.Zero);

        // Act

        var pricing = new OneTimePricing(price);

        // Assert

        pricing.Price.ShouldBe(price);
    }
}
