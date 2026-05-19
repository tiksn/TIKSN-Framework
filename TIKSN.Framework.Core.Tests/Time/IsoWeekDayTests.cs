using System;
using NodaTime;
using NodaTime.Calendars;
using Shouldly;
using TIKSN.Time;
using Xunit;

namespace TIKSN.Tests.Time;

public class IsoWeekDayTests
{
    [Theory]
    [InlineData(2019, 12, 29, 0, 0, 1, false)]
    [InlineData(2019, 12, 30, 0, 0, 1, true)]
    [InlineData(2019, 12, 31, 0, 0, 1, false)]
    public void GivenDateInfo_WhenContainsChecked_ThenResultShouldMatch(
        int year, int month, int day, int hour, int minute, int offsetInHours, bool expectedContains)
    {
        // Arrange

        var isoWeekDay = new IsoWeekDay(2020, 1, IsoDayOfWeek.Monday);

        var localDate = new LocalDate(year, month, day);
        var localDateTime = new LocalDateTime(year, month, day, hour, minute);
        var offsetDateTime = new OffsetDateTime(localDateTime, Offset.FromHours(offsetInHours));
        var offsetDate = new OffsetDate(localDate, Offset.FromHours(2));
        var zonedDateTime = new ZonedDateTime(localDateTime, DateTimeZone.Utc, Offset.Zero);

        // Act

        var containsLocalDate = isoWeekDay.Contains(localDate);
        var containsLocalDateTime = isoWeekDay.Contains(localDateTime);
        var containsOffsetDate = isoWeekDay.Contains(offsetDate);
        var containsOffsetDateTime = isoWeekDay.Contains(offsetDateTime);
        var containsZonedDateTime = isoWeekDay.Contains(zonedDateTime);

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

        var isoWeekDay = new IsoWeekDay(Era.BeforeCommon, weekYearOfEra: 2, 52, IsoDayOfWeek.Sunday);

        // Act

        var actual = isoWeekDay.GetNext();

        // Assert

        actual.IsNone.ShouldBeTrue();
    }

    [Fact]
    public void GivenDay_WhenCompared_ThenResultShouldMatch()
    {
        // Arrange

        var monday = new IsoWeekDay(2020, 1, IsoDayOfWeek.Monday);
        var tuesday = new IsoWeekDay(2020, 1, IsoDayOfWeek.Tuesday);

        // Act and Assert

        (monday < tuesday).ShouldBeTrue();
        monday.Equals(new IsoWeekDay(2020, 1, IsoDayOfWeek.Monday)).ShouldBeTrue();
        monday.ToString().ShouldBe("2020-W01-1");
    }

    [Fact]
    public void GivenDay_WhenConverted_ThenResultShouldMatch()
    {
        // Arrange

        var isoWeekDay = new IsoWeekDay(2020, 53, IsoDayOfWeek.Friday);

        // Act

        var actualDate = isoWeekDay.ToLocalDate();
        var actualInterval = isoWeekDay.ToDateInterval().ToString();

        // Assert

        actualDate.ShouldBe(new LocalDate(2021, 1, 1));
        actualInterval.ShouldBe("[2021-01-01, 2021-01-01]");
    }

    [Theory]
    [InlineData(2020, 1, IsoDayOfWeek.Monday, "[2019-12-29, 2019-12-29]", "[2019-12-31, 2019-12-31]")]
    [InlineData(2020, 53, IsoDayOfWeek.Sunday, "[2021-01-02, 2021-01-02]", "[2021-01-04, 2021-01-04]")]
    public void GivenDay_WhenNextAndPreviousRequested_ThenResultShouldMatch(
        int weekYear, int weekOfWeekYear, IsoDayOfWeek dayOfWeek, string expectedPrevious, string expectedNext)
    {
        // Arrange

        var isoWeekDay = new IsoWeekDay(weekYear, weekOfWeekYear, dayOfWeek);

        // Act

        var actualPrevious = isoWeekDay.GetPrevious().MatchUnsafe(d => d.ToDateInterval().ToString(), () => null);
        var actualNext = isoWeekDay.GetNext().MatchUnsafe(d => d.ToDateInterval().ToString(), () => null);

        // Assert

        actualPrevious.ShouldBe(expectedPrevious);
        actualNext.ShouldBe(expectedNext);
    }

    [Theory]
    [InlineData(0, 1, IsoDayOfWeek.Monday)]
    [InlineData(2021, 53, IsoDayOfWeek.Monday)]
    [InlineData(2020, 1, (IsoDayOfWeek)0)]
    public void GivenInvalidDayInfo_WhenCreated_ThenShouldThrow(
        int weekYear, int weekOfWeekYear, IsoDayOfWeek dayOfWeek)
    {
        // Act
        Action action = () => _ = new IsoWeekDay(weekYear, weekOfWeekYear, dayOfWeek);

        // Assert
        _ = action.ShouldThrow<ArgumentOutOfRangeException>();
    }
}
