using System;
using Shouldly;
using TIKSN.Finance.Pricing;
using TIKSN.Time;
using Xunit;

namespace TIKSN.Tests.Finance.Pricing;

public class YearlyPricingTests
{
    [Fact]
    public void GivenDifferentValues_WhenCompared_ThenShouldNotBeEqual()
    {
        // Arrange

        var price = PricingTestHelper.CreateMoney(10m);
        var first = new YearlyPricing<CalendarYear>(new CalendarYear(2026), price);
        var second = new YearlyPricing<CalendarYear>(new CalendarYear(2027), price);

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

        Action action = () => _ = new YearlyPricing<CalendarYear>(new CalendarYear(2026), price);

        // Assert

        _ = action.ShouldThrow<ArgumentOutOfRangeException>();
    }

    [Fact]
    public void GivenSameValues_WhenCompared_ThenShouldBeEqual()
    {
        // Arrange

        var first = new YearlyPricing<CalendarYear>(
            new CalendarYear(2026),
            PricingTestHelper.CreateMoney(10m));
        var second = new YearlyPricing<CalendarYear>(
            new CalendarYear(2026),
            PricingTestHelper.CreateMoney(10m));

        // Act and Assert

        first.ShouldBe(second);
        (first == second).ShouldBeTrue();
        (first != second).ShouldBeFalse();
    }

    [Fact]
    public void GivenYearlyPricing_WhenCreated_ThenPriceAndYearShouldBeExposed()
    {
        // Arrange

        var year = new CalendarYear(2026);
        var price = PricingTestHelper.CreateMoney(300m);

        // Act

        var pricing = new YearlyPricing<CalendarYear>(year, price);

        // Assert

        pricing.Price.ShouldBe(price);
        pricing.Year.ShouldBe(year);
        ((IYearlyPricing)pricing).Year.ShouldBe(year);
        pricing.Price.ShouldBe(price);
    }

    [Fact]
    public void GivenZeroPrice_WhenCreated_ThenShouldBeAccepted()
    {
        // Arrange

        var price = PricingTestHelper.CreateMoney(decimal.Zero);

        // Act

        var pricing = new YearlyPricing<CalendarYear>(new CalendarYear(2026), price);

        // Assert

        pricing.Price.ShouldBe(price);
    }
}
