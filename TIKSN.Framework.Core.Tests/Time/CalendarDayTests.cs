using System;
using NodaTime;
using Shouldly;
using TIKSN.Time;
using Xunit;

namespace TIKSN.Tests.Time;

public class CalendarDayTests
{
    [Theory]
    [InlineData(2023, 10, 14, 11, 43, 2, true)]
    [InlineData(2023, 10, 13, 10, 30, 2, false)]
    [InlineData(2023, 10, 15, 10, 30, 2, false)]
    public void GivenDateInfo_WhenContainsChecked_ThenResultShouldMatch(
        int year, int month, int day, int hour, int minute, int offsetInHours, bool expectedContains)
    {
        // Arrange

        var calendarDay = new CalendarDay(2023, 10, 14);

        var localDate = new LocalDate(year, month, day);
        var localDateTime = new LocalDateTime(year, month, day, hour, minute);
        var offsetDateTime = new OffsetDateTime(localDateTime, Offset.FromHours(offsetInHours));
        var offsetDate = new OffsetDate(localDate, Offset.FromHours(2));
        var zonedDateTime = new ZonedDateTime(localDateTime, DateTimeZone.Utc, Offset.Zero);

        // Act

        var containsLocalDate = calendarDay.Contains(localDate);
        var containsLocalDateTime = calendarDay.Contains(localDateTime);
        var containsOffsetDate = calendarDay.Contains(offsetDate);
        var containsOffsetDateTime = calendarDay.Contains(offsetDateTime);
        var containsZonedDateTime = calendarDay.Contains(zonedDateTime);

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

        var calendarDay = new CalendarDay(new LocalDate(-1, 12, 31));

        // Act

        var actual = calendarDay.GetNext();

        // Assert

        actual.IsNone.ShouldBeTrue();
    }

    [Fact]
    public void GivenDay_WhenCompared_ThenResultShouldMatch()
    {
        // Arrange

        var october14 = new CalendarDay(2023, 10, 14);
        var october15 = new CalendarDay(2023, 10, 15);

        // Act and Assert

        (october14 < october15).ShouldBeTrue();
        october14.Equals(new CalendarDay(2023, 10, 14)).ShouldBeTrue();
        october14.ToString().ShouldBe("2023-10-14");
    }

    [Fact]
    public void GivenDay_WhenConverted_ThenResultShouldMatch()
    {
        // Arrange

        var calendarDay = new CalendarDay(2024, 2, 29);

        // Act

        var actualDate = calendarDay.ToLocalDate();
        var actualInterval = calendarDay.ToDateInterval().ToString();

        // Assert

        actualDate.ShouldBe(new LocalDate(2024, 2, 29));
        actualInterval.ShouldBe("[2024-02-29, 2024-02-29]");
    }

    [Theory]
    [InlineData(2023, 12, 31, "[2023-12-30, 2023-12-30]", "[2024-01-01, 2024-01-01]")]
    [InlineData(2024, 2, 29, "[2024-02-28, 2024-02-28]", "[2024-03-01, 2024-03-01]")]
    public void GivenDay_WhenNextAndPreviousRequested_ThenResultShouldMatch(
        int year, int month, int day, string expectedPrevious, string expectedNext)
    {
        // Arrange

        var calendarDay = new CalendarDay(year, month, day);

        // Act

        var actualPrevious = calendarDay.GetPrevious().MatchUnsafe(d => d.ToDateInterval().ToString(), () => null);
        var actualNext = calendarDay.GetNext().MatchUnsafe(d => d.ToDateInterval().ToString(), () => null);

        // Assert

        actualPrevious.ShouldBe(expectedPrevious);
        actualNext.ShouldBe(expectedNext);
    }

    [Theory]
    [InlineData(0, 1, 1)]
    [InlineData(2023, 2, 29)]
    public void GivenInvalidDayInfo_WhenCreated_ThenShouldThrow(int year, int month, int day)
    {
        // Act
        Action action = () => _ = new CalendarDay(year, month, day);

        // Assert
        _ = action.ShouldThrow<ArgumentOutOfRangeException>();
    }
}
