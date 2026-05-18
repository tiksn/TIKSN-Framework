using System;
using NodaTime;
using NodaTime.Calendars;
using Shouldly;
using TIKSN.Time;
using Xunit;

namespace TIKSN.Tests.Time;

public class IsoWeekYearTests
{
    [Theory]
    [InlineData(2020, 1, 1, 0, 0, 1, true)]
    [InlineData(2021, 1, 1, 0, 0, 1, true)]
    [InlineData(2021, 1, 4, 0, 0, 1, false)]
    [InlineData(2019, 12, 29, 0, 0, 1, false)]
    [InlineData(2019, 12, 30, 0, 0, 1, true)]
    public void GivenDateInfo_WhenContainsChecked_ThenResultShouldMatch(
        int year, int month, int day, int hour, int minute, int offsetInHours, bool expectedContains)
    {
        // Arrange

        var isoWeekYear2020 = new IsoWeekYear(2020);

        var localDate = new LocalDate(year, month, day);
        var localDateTime = new LocalDateTime(year, month, day, hour, minute);
        var offsetDateTime = new OffsetDateTime(localDateTime, Offset.FromHours(offsetInHours));
        var offsetDate = new OffsetDate(localDate, Offset.FromHours(2));
        var warsaw = DateTimeZoneProviders.Tzdb["Europe/Warsaw"];
        var zonedDateTime = new ZonedDateTime(localDateTime, warsaw, Offset.FromHours(offsetInHours));

        // Act

        var containsLocalDate = isoWeekYear2020.Contains(localDate);
        var containsLocalDateTime = isoWeekYear2020.Contains(localDateTime);
        var containsOffsetDate = isoWeekYear2020.Contains(offsetDate);
        var containsOffsetDateTime = isoWeekYear2020.Contains(offsetDateTime);
        var containsZonedDateTime = isoWeekYear2020.Contains(zonedDateTime);

        // Assert

        containsLocalDate.ShouldBe(expectedContains);
        containsLocalDateTime.ShouldBe(expectedContains);
        containsOffsetDate.ShouldBe(expectedContains);
        containsOffsetDateTime.ShouldBe(expectedContains);
        containsZonedDateTime.ShouldBe(expectedContains);
    }

    [Fact]
    public void GivenYearBeforeCommonEra_WhenNextWouldBeZero_ThenNoneShouldBeReturned()
    {
        // Arrange

        var isoWeekYear = new IsoWeekYear(Era.BeforeCommon, weekYearOfEra: 2);

        // Act

        var actual = isoWeekYear.GetNext();

        // Assert

        actual.IsNone.ShouldBeTrue();
    }

    [Theory]
    [InlineData(2019, 12, true)]
    [InlineData(2020, 6, true)]
    [InlineData(2021, 1, true)]
    [InlineData(2021, 2, false)]
    public void GivenYearMonth_WhenContainsChecked_ThenOverlapResultShouldMatch(
        int year, int month, bool expectedContains)
    {
        // Arrange

        var isoWeekYear2020 = new IsoWeekYear(2020);
        var yearMonth = new YearMonth(year, month);

        // Act

        var actual = isoWeekYear2020.Contains(yearMonth);

        // Assert

        actual.ShouldBe(expectedContains);
    }

    [Theory]
    [InlineData(2020, "2019", "2021")]
    [InlineData(2021, "2020", "2022")]
    [InlineData(2022, "2021", "2023")]
    public void GivenYear_WhenNextAndPreviousRequested_ThenResultShouldMatch(
        int year, string expectedPrevious, string expectedNext)
    {
        // Arrange

        var isoWeekYear = new IsoWeekYear(year);

        // Act

        var actualPrevious = isoWeekYear.GetPrevious().MatchUnsafe(y => y.ToString(), () => null);
        var actualNext = isoWeekYear.GetNext().MatchUnsafe(y => y.ToString(), () => null);

        // Assert

        actualPrevious.ShouldBe(expectedPrevious);
        actualNext.ShouldBe(expectedNext);
    }

    [Theory]
    [InlineData(2020, "[2019-12-30, 2021-01-03]")]
    [InlineData(2021, "[2021-01-04, 2022-01-02]")]
    [InlineData(2022, "[2022-01-03, 2023-01-01]")]
    public void GivenYear_WhenToDateInterval_ThenResultShouldMatch(
        int year, string expected)
    {
        // Arrange

        var isoWeekYear = new IsoWeekYear(year);

        // Act

        var actual = isoWeekYear.ToDateInterval().ToString();

        // Assert

        actual.ShouldBe(expected);
    }

    [Theory]
    [InlineData(2020, "2020")]
    [InlineData(2021, "2021")]
    [InlineData(2022, "2022")]
    public void GivenYear_WhenToString_ThenResultShouldMatch(
        int year, string expected)
    {
        // Arrange

        var isoWeekYear = new IsoWeekYear(year);

        // Act

        var actual = isoWeekYear.ToString();

        // Assert

        actual.ShouldBe(expected);
    }

    [Fact]
    public void GivenZeroYear_WhenCreated_ThenShouldThrow()
    {
        // Act
        Action action = () => _ = new IsoWeekYear(0);

        // Assert
        action.ShouldThrow<ArgumentOutOfRangeException>();
    }
}
