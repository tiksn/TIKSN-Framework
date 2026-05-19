using System;
using NodaTime;
using Shouldly;
using TIKSN.Time;
using Xunit;

namespace TIKSN.Tests.Time;

public class CycleMonthTests
{
    [Fact]
    public void GivenCycleMonth_WhenNextAndPreviousRequested_ThenResultShouldMatch()
    {
        // Arrange

        var cycleMonth = new CycleMonth(new YearMonth(2026, 12), 15);

        // Act

        var actualPrevious = cycleMonth.GetPrevious().MatchUnsafe(
            m => m.ToDateInterval().ToString(),
            () => null);
        var actualNext = cycleMonth.GetNext().MatchUnsafe(
            m => m.ToDateInterval().ToString(),
            () => null);

        // Assert

        actualPrevious.ShouldBe("[2026-11-15, 2026-12-14]");
        actualNext.ShouldBe("[2027-01-15, 2027-02-14]");
    }

    [Theory]
    [InlineData(2026, 5, 15, "[2026-05-15, 2026-06-14]")]
    [InlineData(2025, 2, 31, "[2025-02-28, 2025-03-30]")]
    [InlineData(2024, 2, 31, "[2024-02-29, 2024-03-30]")]
    public void GivenCycleMonth_WhenToDateInterval_ThenResultShouldMatch(
        int year, int month, int startDayOfMonth, string expected)
    {
        // Arrange

        var cycleMonth = new CycleMonth(new YearMonth(year, month), startDayOfMonth);

        // Act

        var actual = cycleMonth.ToDateInterval().ToString();

        // Assert

        actual.ShouldBe(expected);
    }

    [Theory]
    [InlineData(2026, 5, 14, 11, 43, 2, false)]
    [InlineData(2026, 5, 15, 11, 43, 2, true)]
    [InlineData(2026, 6, 14, 10, 30, 2, true)]
    [InlineData(2026, 6, 15, 10, 30, 2, false)]
    public void GivenDateInfo_WhenContainsChecked_ThenResultShouldMatch(
        int year, int month, int day, int hour, int minute, int offsetInHours, bool expectedContains)
    {
        // Arrange

        var cycleMonth = new CycleMonth(new YearMonth(2026, 5), 15);

        var localDate = new LocalDate(year, month, day);
        var localDateTime = new LocalDateTime(year, month, day, hour, minute);
        var offsetDateTime = new OffsetDateTime(localDateTime, Offset.FromHours(offsetInHours));
        var offsetDate = new OffsetDate(localDate, Offset.FromHours(2));
        var zonedDateTime = new ZonedDateTime(localDateTime, DateTimeZone.Utc, Offset.Zero);

        // Act

        var containsLocalDate = cycleMonth.Contains(localDate);
        var containsLocalDateTime = cycleMonth.Contains(localDateTime);
        var containsOffsetDate = cycleMonth.Contains(offsetDate);
        var containsOffsetDateTime = cycleMonth.Contains(offsetDateTime);
        var containsZonedDateTime = cycleMonth.Contains(zonedDateTime);

        // Assert

        containsLocalDate.ShouldBe(expectedContains);
        containsLocalDateTime.ShouldBe(expectedContains);
        containsOffsetDate.ShouldBe(expectedContains);
        containsOffsetDateTime.ShouldBe(expectedContains);
        containsZonedDateTime.ShouldBe(expectedContains);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(32)]
    public void GivenInvalidStartDay_WhenCreated_ThenShouldThrow(int startDayOfMonth)
    {
        // Act
        Action action = () => _ = new CycleMonth(new YearMonth(2026, 5), startDayOfMonth);

        // Assert
        _ = action.ShouldThrow<ArgumentOutOfRangeException>();
    }

    [Fact]
    public void GivenMonthBeforeCommonEra_WhenNextWouldBeYearZero_ThenNoneShouldBeReturned()
    {
        // Arrange

        var cycleMonth = new CycleMonth(new YearMonth(-1, 12), 15);

        // Act

        var actual = cycleMonth.GetNext();

        // Assert

        actual.IsNone.ShouldBeTrue();
    }

    [Fact]
    public void GivenMonth_WhenCompared_ThenResultShouldMatch()
    {
        // Arrange

        var may = new CycleMonth(new YearMonth(2026, 5), 15);
        var june = new CycleMonth(new YearMonth(2026, 6), 15);

        // Act and Assert

        (may < june).ShouldBeTrue();
        may.Equals(new CycleMonth(new YearMonth(2026, 5), 15)).ShouldBeTrue();
        may.ToString().ShouldBe("2026-05-15");
    }

    [Theory]
    [InlineData(2026, 4, false)]
    [InlineData(2026, 5, true)]
    [InlineData(2026, 6, true)]
    [InlineData(2026, 7, false)]
    public void GivenYearMonth_WhenContainsChecked_ThenOverlapResultShouldMatch(
        int year, int month, bool expectedContains)
    {
        // Arrange

        var cycleMonth = new CycleMonth(new YearMonth(2026, 5), 15);

        // Act

        var actual = cycleMonth.Contains(new YearMonth(year, month));

        // Assert

        actual.ShouldBe(expectedContains);
    }
}
