using System;
using NodaTime;
using NodaTime.Calendars;
using Shouldly;
using TIKSN.Time;
using Xunit;

namespace TIKSN.Tests.Time;

public class FiscalMonthTests
{
    [Theory]
    [InlineData(2023, 10, 14, 11, 43, 2, false)]
    [InlineData(2023, 10, 15, 11, 43, 2, true)]
    [InlineData(2023, 11, 14, 10, 30, 2, true)]
    [InlineData(2023, 11, 15, 10, 30, 2, false)]
    public void GivenDateInfo_WhenContainsChecked_ThenResultShouldMatch(
        int year, int month, int day, int hour, int minute, int offsetInHours, bool expectedContains)
    {
        // Arrange

        var fiscalMonth = new FiscalMonth(
            new FiscalYear(startYear: 2023, new AnnualDate(month: 10, day: 15)),
            monthOfFiscalYear: 1);

        var localDate = new LocalDate(year, month, day);
        var localDateTime = new LocalDateTime(year, month, day, hour, minute);
        var offsetDateTime = new OffsetDateTime(localDateTime, Offset.FromHours(offsetInHours));
        var offsetDate = new OffsetDate(localDate, Offset.FromHours(2));
        var zonedDateTime = new ZonedDateTime(localDateTime, DateTimeZone.Utc, Offset.Zero);

        // Act

        var containsLocalDate = fiscalMonth.Contains(localDate);
        var containsLocalDateTime = fiscalMonth.Contains(localDateTime);
        var containsOffsetDate = fiscalMonth.Contains(offsetDate);
        var containsOffsetDateTime = fiscalMonth.Contains(offsetDateTime);
        var containsZonedDateTime = fiscalMonth.Contains(zonedDateTime);

        // Assert

        containsLocalDate.ShouldBe(expectedContains);
        containsLocalDateTime.ShouldBe(expectedContains);
        containsOffsetDate.ShouldBe(expectedContains);
        containsOffsetDateTime.ShouldBe(expectedContains);
        containsZonedDateTime.ShouldBe(expectedContains);
    }

    [Theory]
    [InlineData(2023, 9, false)]
    [InlineData(2023, 10, true)]
    [InlineData(2023, 11, true)]
    [InlineData(2023, 12, false)]
    public void GivenYearMonth_WhenContainsChecked_ThenOverlapResultShouldMatch(
        int year, int month, bool expectedContains)
    {
        // Arrange

        var fiscalMonth = new FiscalMonth(
            new FiscalYear(startYear: 2023, new AnnualDate(month: 10, day: 15)),
            monthOfFiscalYear: 1);

        // Act

        var actual = fiscalMonth.Contains(new YearMonth(year, month));

        // Assert

        actual.ShouldBe(expectedContains);
    }

    [Fact]
    public void GivenMonthBeforeCommonEra_WhenNextWouldBeYearZero_ThenNoneShouldBeReturned()
    {
        // Arrange

        var fiscalMonth = new FiscalMonth(
            new FiscalYear(Era.BeforeCommon, startYearOfEra: 2, new AnnualDate(month: 1, day: 1)),
            monthOfFiscalYear: 12);

        // Act

        var actual = fiscalMonth.GetNext();

        // Assert

        actual.IsNone.ShouldBeTrue();
    }

    [Fact]
    public void GivenMonth_WhenCompared_ThenResultShouldMatch()
    {
        // Arrange

        var firstMonth = new FiscalMonth(new FiscalYear(2023, new AnnualDate(month: 10, day: 1)), 1);
        var secondMonth = new FiscalMonth(new FiscalYear(2023, new AnnualDate(month: 10, day: 1)), 2);

        // Act and Assert

        (firstMonth < secondMonth).ShouldBeTrue();
        firstMonth.Equals(new FiscalMonth(new FiscalYear(2023, new AnnualDate(month: 10, day: 1)), 1)).ShouldBeTrue();
        firstMonth.ToString().ShouldBe("2023/2024-01");
    }

    [Theory]
    [InlineData(1, "[2023-09-01, 2023-09-30]", "[2023-11-01, 2023-11-30]")]
    [InlineData(12, "[2024-08-01, 2024-08-31]", "[2024-10-01, 2024-10-31]")]
    public void GivenMonth_WhenNextAndPreviousRequested_ThenResultShouldMatch(
        int monthOfFiscalYear, string expectedPrevious, string expectedNext)
    {
        // Arrange

        var fiscalMonth = new FiscalMonth(
            new FiscalYear(startYear: 2023, new AnnualDate(month: 10, day: 1)),
            monthOfFiscalYear);

        // Act

        var actualPrevious = fiscalMonth.GetPrevious().MatchUnsafe(
            m => m.ToDateInterval().ToString(),
            () => null);
        var actualNext = fiscalMonth.GetNext().MatchUnsafe(
            m => m.ToDateInterval().ToString(),
            () => null);

        // Assert

        actualPrevious.ShouldBe(expectedPrevious);
        actualNext.ShouldBe(expectedNext);
    }

    [Theory]
    [InlineData(1, "[2023-10-01, 2023-10-31]")]
    [InlineData(12, "[2024-09-01, 2024-09-30]")]
    public void GivenMonth_WhenToDateInterval_ThenResultShouldMatch(int monthOfFiscalYear, string expected)
    {
        // Arrange

        var fiscalMonth = new FiscalMonth(
            new FiscalYear(startYear: 2023, new AnnualDate(month: 10, day: 1)),
            monthOfFiscalYear);

        // Act

        var actual = fiscalMonth.ToDateInterval().ToString();

        // Assert

        actual.ShouldBe(expected);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(13)]
    public void GivenInvalidMonthOrdinal_WhenCreated_ThenShouldThrow(int monthOfFiscalYear)
    {
        // Act
        Action action = () => _ = new FiscalMonth(new FiscalYear(2023, new AnnualDate(month: 10, day: 1)), monthOfFiscalYear);

        // Assert
        action.ShouldThrow<ArgumentOutOfRangeException>();
    }
}
