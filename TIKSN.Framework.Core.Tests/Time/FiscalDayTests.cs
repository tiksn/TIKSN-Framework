using System;
using NodaTime;
using NodaTime.Calendars;
using Shouldly;
using TIKSN.Time;
using Xunit;

namespace TIKSN.Tests.Time;

public class FiscalDayTests
{
    [Theory]
    [InlineData(2023, 10, 14, 11, 43, 2, false)]
    [InlineData(2023, 10, 15, 11, 43, 2, true)]
    [InlineData(2023, 10, 16, 10, 30, 2, false)]
    public void GivenDateInfo_WhenContainsChecked_ThenResultShouldMatch(
        int year, int month, int day, int hour, int minute, int offsetInHours, bool expectedContains)
    {
        // Arrange

        var fiscalDay = new FiscalDay(new FiscalYear(2023, new AnnualDate(month: 10, day: 1)), 15);

        var localDate = new LocalDate(year, month, day);
        var localDateTime = new LocalDateTime(year, month, day, hour, minute);
        var offsetDateTime = new OffsetDateTime(localDateTime, Offset.FromHours(offsetInHours));
        var offsetDate = new OffsetDate(localDate, Offset.FromHours(2));
        var zonedDateTime = new ZonedDateTime(localDateTime, DateTimeZone.Utc, Offset.Zero);

        // Act

        var containsLocalDate = fiscalDay.Contains(localDate);
        var containsLocalDateTime = fiscalDay.Contains(localDateTime);
        var containsOffsetDate = fiscalDay.Contains(offsetDate);
        var containsOffsetDateTime = fiscalDay.Contains(offsetDateTime);
        var containsZonedDateTime = fiscalDay.Contains(zonedDateTime);

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

        var fiscalDay =
            new FiscalDay(new FiscalYear(Era.BeforeCommon, startYearOfEra: 2, new AnnualDate(month: 1, day: 1)), 365);

        // Act

        var actual = fiscalDay.GetNext();

        // Assert

        actual.IsNone.ShouldBeTrue();
    }

    [Fact]
    public void GivenDay_WhenCompared_ThenResultShouldMatch()
    {
        // Arrange

        var firstDay = new FiscalDay(new FiscalYear(2023, new AnnualDate(month: 10, day: 1)), 1);
        var secondDay = new FiscalDay(new FiscalYear(2023, new AnnualDate(month: 10, day: 1)), 2);

        // Act and Assert

        (firstDay < secondDay).ShouldBeTrue();
        firstDay.Equals(new FiscalDay(new FiscalYear(2023, new AnnualDate(month: 10, day: 1)), 1)).ShouldBeTrue();
        firstDay.ToString().ShouldBe("2023/2024-01");
    }

    [Fact]
    public void GivenDay_WhenConverted_ThenResultShouldMatch()
    {
        // Arrange

        var fiscalDay = new FiscalDay(new FiscalYear(startYear: 2023, new AnnualDate(month: 10, day: 1)), 153);

        // Act

        var actualDate = fiscalDay.ToLocalDate();
        var actualInterval = fiscalDay.ToDateInterval().ToString();

        // Assert

        actualDate.ShouldBe(new LocalDate(2024, 3, 1));
        actualInterval.ShouldBe("[2024-03-01, 2024-03-01]");
    }

    [Theory]
    [InlineData(1, "[2023-09-30, 2023-09-30]", "[2023-10-02, 2023-10-02]")]
    [InlineData(366, "[2024-09-29, 2024-09-29]", "[2024-10-01, 2024-10-01]")]
    public void GivenDay_WhenNextAndPreviousRequested_ThenResultShouldMatch(
        int dayOfFiscalYear, string expectedPrevious, string expectedNext)
    {
        // Arrange

        var fiscalDay = new FiscalDay(new FiscalYear(startYear: 2023, new AnnualDate(month: 10, day: 1)),
            dayOfFiscalYear);

        // Act

        var actualPrevious = fiscalDay.GetPrevious().MatchUnsafe(d => d.ToDateInterval().ToString(), () => null);
        var actualNext = fiscalDay.GetNext().MatchUnsafe(d => d.ToDateInterval().ToString(), () => null);

        // Assert

        actualPrevious.ShouldBe(expectedPrevious);
        actualNext.ShouldBe(expectedNext);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(367)]
    public void GivenInvalidDayOrdinal_WhenCreated_ThenShouldThrow(int dayOfFiscalYear)
    {
        // Act
        Action action = () =>
            _ = new FiscalDay(new FiscalYear(2023, new AnnualDate(month: 10, day: 1)), dayOfFiscalYear);

        // Assert
        _ = action.ShouldThrow<ArgumentOutOfRangeException>();
    }
}
