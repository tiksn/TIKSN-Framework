using System;
using NodaTime;
using Shouldly;
using TIKSN.Time;
using Xunit;

namespace TIKSN.Tests.Time;

public class CycleDayTests
{
    [Theory]
    [InlineData(2025, 2, 31, 1, "2025-02-28", "[2025-02-28, 2025-02-28]")]
    [InlineData(2024, 2, 31, 31, "2024-03-30", "[2024-03-30, 2024-03-30]")]
    public void GivenClampedCycleDay_WhenConverted_ThenResultShouldMatch(
        int year, int month, int startDayOfMonth, int dayOfCycleMonth, string expectedDate, string expectedInterval)
    {
        // Arrange

        var cycleDay = new CycleDay(new CycleMonth(new YearMonth(year, month), startDayOfMonth), dayOfCycleMonth);

        // Act

        var actualDate = cycleDay.ToLocalDate().ToString("yyyy-MM-dd", null);
        var actualInterval = cycleDay.ToDateInterval().ToString();

        // Assert

        actualDate.ShouldBe(expectedDate);
        actualInterval.ShouldBe(expectedInterval);
    }

    [Theory]
    [InlineData(2026, 5, 14, 11, 43, 2, false)]
    [InlineData(2026, 5, 15, 11, 43, 2, true)]
    [InlineData(2026, 5, 16, 10, 30, 2, false)]
    public void GivenDateInfo_WhenContainsChecked_ThenResultShouldMatch(
        int year, int month, int day, int hour, int minute, int offsetInHours, bool expectedContains)
    {
        // Arrange

        var cycleDay = new CycleDay(new CycleMonth(new YearMonth(2026, 5), 15), 1);

        var localDate = new LocalDate(year, month, day);
        var localDateTime = new LocalDateTime(year, month, day, hour, minute);
        var offsetDateTime = new OffsetDateTime(localDateTime, Offset.FromHours(offsetInHours));
        var offsetDate = new OffsetDate(localDate, Offset.FromHours(2));
        var zonedDateTime = new ZonedDateTime(localDateTime, DateTimeZone.Utc, Offset.Zero);

        // Act

        var containsLocalDate = cycleDay.Contains(localDate);
        var containsLocalDateTime = cycleDay.Contains(localDateTime);
        var containsOffsetDate = cycleDay.Contains(offsetDate);
        var containsOffsetDateTime = cycleDay.Contains(offsetDateTime);
        var containsZonedDateTime = cycleDay.Contains(zonedDateTime);

        // Assert

        containsLocalDate.ShouldBe(expectedContains);
        containsLocalDateTime.ShouldBe(expectedContains);
        containsOffsetDate.ShouldBe(expectedContains);
        containsOffsetDateTime.ShouldBe(expectedContains);
        containsZonedDateTime.ShouldBe(expectedContains);
    }

    [Fact]
    public void GivenDayBeforeCommonEra_WhenNextWouldBeYearZero_ThenNoneShouldBeReturned()
    {
        // Arrange

        var cycleDay = new CycleDay(new CycleMonth(new YearMonth(-1, 12), 1), 31);

        // Act

        var actual = cycleDay.GetNext();

        // Assert

        actual.IsNone.ShouldBeTrue();
    }

    [Fact]
    public void GivenDay_WhenCompared_ThenResultShouldMatch()
    {
        // Arrange

        var firstDay = new CycleDay(new CycleMonth(new YearMonth(2026, 5), 15), 1);
        var secondDay = new CycleDay(new CycleMonth(new YearMonth(2026, 5), 15), 2);

        // Act and Assert

        (firstDay < secondDay).ShouldBeTrue();
        firstDay.Equals(new CycleDay(new CycleMonth(new YearMonth(2026, 5), 15), 1)).ShouldBeTrue();
        firstDay.ToString().ShouldBe("2026-05-15-01");
    }

    [Theory]
    [InlineData(1, "[2026-05-14, 2026-05-14]", "[2026-05-16, 2026-05-16]")]
    [InlineData(31, "[2026-06-13, 2026-06-13]", "[2026-06-15, 2026-06-15]")]
    public void GivenDay_WhenNextAndPreviousRequested_ThenResultShouldMatch(
        int dayOfCycleMonth, string expectedPrevious, string expectedNext)
    {
        // Arrange

        var cycleDay = new CycleDay(new CycleMonth(new YearMonth(2026, 5), 15), dayOfCycleMonth);

        // Act

        var actualPrevious = cycleDay.GetPrevious().MatchUnsafe(d => d.ToDateInterval().ToString(), () => null);
        var actualNext = cycleDay.GetNext().MatchUnsafe(d => d.ToDateInterval().ToString(), () => null);

        // Assert

        actualPrevious.ShouldBe(expectedPrevious);
        actualNext.ShouldBe(expectedNext);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(32)]
    public void GivenInvalidDayOrdinal_WhenCreated_ThenShouldThrow(int dayOfCycleMonth)
    {
        // Act
        Action action = () => _ = new CycleDay(new CycleMonth(new YearMonth(2026, 5), 15), dayOfCycleMonth);

        // Assert
        _ = action.ShouldThrow<ArgumentOutOfRangeException>();
    }
}
