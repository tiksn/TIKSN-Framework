using System;
using NodaTime;
using NodaTime.Calendars;
using Shouldly;
using TIKSN.Time;
using Xunit;

namespace TIKSN.Tests.Time;

public class AcademicDayTests
{
    [Theory]
    [InlineData(2023, 8, 31, 11, 43, 2, false)]
    [InlineData(2023, 9, 1, 11, 43, 2, true)]
    [InlineData(2023, 9, 2, 10, 30, 2, false)]
    public void GivenDateInfo_WhenContainsChecked_ThenResultShouldMatch(
        int year, int month, int day, int hour, int minute, int offsetInHours, bool expectedContains)
    {
        // Arrange

        var academicDay = new AcademicDay(new AcademicYear(2023), 1);

        var localDate = new LocalDate(year, month, day);
        var localDateTime = new LocalDateTime(year, month, day, hour, minute);
        var offsetDateTime = new OffsetDateTime(localDateTime, Offset.FromHours(offsetInHours));
        var offsetDate = new OffsetDate(localDate, Offset.FromHours(2));
        var zonedDateTime = new ZonedDateTime(localDateTime, DateTimeZone.Utc, Offset.Zero);

        // Act

        var containsLocalDate = academicDay.Contains(localDate);
        var containsLocalDateTime = academicDay.Contains(localDateTime);
        var containsOffsetDate = academicDay.Contains(offsetDate);
        var containsOffsetDateTime = academicDay.Contains(offsetDateTime);
        var containsZonedDateTime = academicDay.Contains(zonedDateTime);

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

        var academicDay = new AcademicDay(new AcademicYear(Era.BeforeCommon, startYearOfEra: 2), 365);

        // Act

        var actual = academicDay.GetNext();

        // Assert

        actual.IsNone.ShouldBeTrue();
    }

    [Fact]
    public void GivenDay_WhenCompared_ThenResultShouldMatch()
    {
        // Arrange

        var firstDay = new AcademicDay(new AcademicYear(2023), 1);
        var secondDay = new AcademicDay(new AcademicYear(2023), 2);

        // Act and Assert

        (firstDay < secondDay).ShouldBeTrue();
        firstDay.Equals(new AcademicDay(new AcademicYear(2023), 1)).ShouldBeTrue();
        firstDay.ToString().ShouldBe("2023/2024-01");
    }

    [Fact]
    public void GivenDay_WhenConverted_ThenResultShouldMatch()
    {
        // Arrange

        var academicDay = new AcademicDay(new AcademicYear(2023), 183);

        // Act

        var actualDate = academicDay.ToLocalDate();
        var actualInterval = academicDay.ToDateInterval().ToString();

        // Assert

        actualDate.ShouldBe(new LocalDate(2024, 3, 1));
        actualInterval.ShouldBe("[2024-03-01, 2024-03-01]");
    }

    [Theory]
    [InlineData(1, "[2023-08-31, 2023-08-31]", "[2023-09-02, 2023-09-02]")]
    [InlineData(366, "[2024-08-30, 2024-08-30]", "[2024-09-01, 2024-09-01]")]
    public void GivenDay_WhenNextAndPreviousRequested_ThenResultShouldMatch(
        int dayOfAcademicYear, string expectedPrevious, string expectedNext)
    {
        // Arrange

        var academicDay = new AcademicDay(new AcademicYear(2023), dayOfAcademicYear);

        // Act

        var actualPrevious = academicDay.GetPrevious().MatchUnsafe(d => d.ToDateInterval().ToString(), () => null);
        var actualNext = academicDay.GetNext().MatchUnsafe(d => d.ToDateInterval().ToString(), () => null);

        // Assert

        actualPrevious.ShouldBe(expectedPrevious);
        actualNext.ShouldBe(expectedNext);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(367)]
    public void GivenInvalidDayOrdinal_WhenCreated_ThenShouldThrow(int dayOfAcademicYear)
    {
        // Act
        Action action = () => _ = new AcademicDay(new AcademicYear(2023), dayOfAcademicYear);

        // Assert
        _ = action.ShouldThrow<ArgumentOutOfRangeException>();
    }
}
