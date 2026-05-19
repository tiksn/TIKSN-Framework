using System;
using Shouldly;
using TIKSN.Finance.Pricing;
using TIKSN.Time;
using Xunit;

namespace TIKSN.Tests.Finance.Pricing;

public class DailyPricingTests
{
    [Fact]
    public void GivenDailyPricing_WhenCreated_ThenPriceAndDayShouldBeExposed()
    {
        // Arrange

        var day = new CalendarDay(2026, 5, 19);
        var price = PricingTestHelper.CreateMoney(3m);

        // Act

        var pricing = new DailyPricing<CalendarDay>(day, price);

        // Assert

        pricing.Price.ShouldBe(price);
        pricing.Day.ShouldBe(day);
        ((IDailyPricing)pricing).Day.ShouldBe(day);
        pricing.Price.ShouldBe(price);
    }

    [Fact]
    public void GivenDifferentValues_WhenCompared_ThenShouldNotBeEqual()
    {
        // Arrange

        var price = PricingTestHelper.CreateMoney(10m);
        var first = new DailyPricing<CalendarDay>(new CalendarDay(2026, 5, 19), price);
        var second = new DailyPricing<CalendarDay>(new CalendarDay(2026, 5, 20), price);

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

        Action action = () => _ = new DailyPricing<CalendarDay>(new CalendarDay(2026, 5, 19), price);

        // Assert

        _ = action.ShouldThrow<ArgumentOutOfRangeException>();
    }

    [Fact]
    public void GivenSameValues_WhenCompared_ThenShouldBeEqual()
    {
        // Arrange

        var first = new DailyPricing<CalendarDay>(
            new CalendarDay(2026, 5, 19),
            PricingTestHelper.CreateMoney(10m));
        var second = new DailyPricing<CalendarDay>(
            new CalendarDay(2026, 5, 19),
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

        var pricing = new DailyPricing<CalendarDay>(new CalendarDay(2026, 5, 19), price);

        // Assert

        pricing.Price.ShouldBe(price);
    }
}
