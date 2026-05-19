using System;
using Shouldly;
using TIKSN.Finance.Pricing;
using TIKSN.Time;
using Xunit;

namespace TIKSN.Tests.Finance.Pricing;

public class MonthlyPricingTests
{
    [Fact]
    public void GivenDifferentValues_WhenCompared_ThenShouldNotBeEqual()
    {
        // Arrange

        var price = PricingTestHelper.CreateMoney(10m);
        var first = new MonthlyPricing<CalendarMonth>(new CalendarMonth(2026, 5), price);
        var second = new MonthlyPricing<CalendarMonth>(new CalendarMonth(2026, 6), price);

        // Act and Assert

        first.ShouldNotBe(second);
        (first == second).ShouldBeFalse();
        (first != second).ShouldBeTrue();
    }

    [Fact]
    public void GivenMonthlyPricing_WhenCreated_ThenPriceAndMonthShouldBeExposed()
    {
        // Arrange

        var month = new CalendarMonth(2026, 5);
        var price = PricingTestHelper.CreateMoney(30m);

        // Act

        var pricing = new MonthlyPricing<CalendarMonth>(month, price);

        // Assert

        pricing.Price.ShouldBe(price);
        pricing.Month.ShouldBe(month);
        ((IMonthlyPricing)pricing).Month.ShouldBe(month);
        pricing.Price.ShouldBe(price);
    }

    [Fact]
    public void GivenNegativePrice_WhenCreated_ThenShouldThrow()
    {
        // Arrange

        var price = PricingTestHelper.CreateMoney(-1m);

        // Act

        Action action = () => _ = new MonthlyPricing<CalendarMonth>(new CalendarMonth(2026, 5), price);

        // Assert

        _ = action.ShouldThrow<ArgumentOutOfRangeException>();
    }

    [Fact]
    public void GivenSameValues_WhenCompared_ThenShouldBeEqual()
    {
        // Arrange

        var first = new MonthlyPricing<CalendarMonth>(
            new CalendarMonth(2026, 5),
            PricingTestHelper.CreateMoney(10m));
        var second = new MonthlyPricing<CalendarMonth>(
            new CalendarMonth(2026, 5),
            PricingTestHelper.CreateMoney(10m));

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

        var pricing = new MonthlyPricing<CalendarMonth>(new CalendarMonth(2026, 5), price);

        // Assert

        pricing.Price.ShouldBe(price);
    }
}
