using System;
using NodaTime;
using Shouldly;
using TIKSN.Time;
using Xunit;

namespace TIKSN.Tests.Time;

public class CalendarMonthTests
{
    [Theory]
    [InlineData(2023, 10, 14, 11, 43, 2, true)]
    [InlineData(2023, 9, 30, 10, 30, 2, false)]
    [InlineData(2023, 11, 1, 10, 30, 2, false)]
    public void GivenDateInfo_WhenContainsChecked_ThenResultShouldMatch(
        int year, int month, int day, int hour, int minute, int offsetInHours, bool expectedContains)
    {
        // Arrange

        var calendarMonth = new CalendarMonth(2023, 10);

        var yearMonth = new YearMonth(year, month);
        var localDate = new LocalDate(year, month, day);
        var localDateTime = new LocalDateTime(year, month, day, hour, minute);
        var offsetDateTime = new OffsetDateTime(localDateTime, Offset.FromHours(offsetInHours));
        var offsetDate = new OffsetDate(localDate, Offset.FromHours(2));
        var zonedDateTime = new ZonedDateTime(localDateTime, DateTimeZone.Utc, Offset.Zero);

        // Act

        var containsYearMonth = calendarMonth.Contains(yearMonth);
        var containsLocalDate = calendarMonth.Contains(localDate);
        var containsLocalDateTime = calendarMonth.Contains(localDateTime);
        var containsOffsetDate = calendarMonth.Contains(offsetDate);
        var containsOffsetDateTime = calendarMonth.Contains(offsetDateTime);
        var containsZonedDateTime = calendarMonth.Contains(zonedDateTime);

        // Assert

        containsYearMonth.ShouldBe(expectedContains);
        containsLocalDate.ShouldBe(expectedContains);
        containsLocalDateTime.ShouldBe(expectedContains);
        containsOffsetDate.ShouldBe(expectedContains);
        containsOffsetDateTime.ShouldBe(expectedContains);
        containsZonedDateTime.ShouldBe(expectedContains);
    }

    [Fact]
    public void GivenMonthBeforeCommonEra_WhenNextWouldBeYearZero_ThenNoneShouldBeReturned()
    {
        // Arrange

        var calendarMonth = new CalendarMonth(new YearMonth(-1, 12));

        // Act

        var actual = calendarMonth.GetNext();

        // Assert

        actual.IsNone.ShouldBeTrue();
    }

    [Fact]
    public void GivenMonth_WhenCompared_ThenResultShouldMatch()
    {
        // Arrange

        var october = new CalendarMonth(2023, 10);
        var november = new CalendarMonth(2023, 11);

        // Act and Assert

        (october < november).ShouldBeTrue();
        october.Equals(new CalendarMonth(2023, 10)).ShouldBeTrue();
        october.ToString().ShouldBe("2023-10");
    }

    [Theory]
    [InlineData(2023, 12, "[2023-11-01, 2023-11-30]", "[2024-01-01, 2024-01-31]")]
    [InlineData(2024, 2, "[2024-01-01, 2024-01-31]", "[2024-03-01, 2024-03-31]")]
    public void GivenMonth_WhenNextAndPreviousRequested_ThenResultShouldMatch(
        int year, int month, string expectedPrevious, string expectedNext)
    {
        // Arrange

        var calendarMonth = new CalendarMonth(year, month);

        // Act

        var actualPrevious = calendarMonth.GetPrevious().MatchUnsafe(
            m => m.ToDateInterval().ToString(),
            () => null);
        var actualNext = calendarMonth.GetNext().MatchUnsafe(
            m => m.ToDateInterval().ToString(),
            () => null);

        // Assert

        actualPrevious.ShouldBe(expectedPrevious);
        actualNext.ShouldBe(expectedNext);
    }

    [Theory]
    [InlineData(2023, 2, "[2023-02-01, 2023-02-28]")]
    [InlineData(2024, 2, "[2024-02-01, 2024-02-29]")]
    public void GivenMonth_WhenToDateInterval_ThenResultShouldMatch(int year, int month, string expected)
    {
        // Arrange

        var calendarMonth = new CalendarMonth(year, month);

        // Act

        var actual = calendarMonth.ToDateInterval().ToString();

        // Assert

        actual.ShouldBe(expected);
    }

    [Theory]
    [InlineData(0, 1)]
    [InlineData(2023, 0)]
    [InlineData(2023, 13)]
    public void GivenInvalidMonthInfo_WhenCreated_ThenShouldThrow(int year, int month)
    {
        // Act
        Action action = () => _ = new CalendarMonth(year, month);

        // Assert
        action.ShouldThrow<ArgumentOutOfRangeException>();
    }
}
