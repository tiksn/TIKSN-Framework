using System;
using NodaTime;
using NodaTime.Calendars;
using Shouldly;
using TIKSN.Time;
using Xunit;

namespace TIKSN.Tests.Time;

public class AcademicMonthTests
{
    [Theory]
    [InlineData(2023, 8, 31, 11, 43, 2, false)]
    [InlineData(2023, 9, 1, 11, 43, 2, true)]
    [InlineData(2023, 9, 30, 10, 30, 2, true)]
    [InlineData(2023, 10, 1, 10, 30, 2, false)]
    public void GivenDateInfo_WhenContainsChecked_ThenResultShouldMatch(
        int year, int month, int day, int hour, int minute, int offsetInHours, bool expectedContains)
    {
        // Arrange

        var academicMonth = new AcademicMonth(new AcademicYear(2023), monthOfAcademicYear: 1);

        var localDate = new LocalDate(year, month, day);
        var localDateTime = new LocalDateTime(year, month, day, hour, minute);
        var offsetDateTime = new OffsetDateTime(localDateTime, Offset.FromHours(offsetInHours));
        var offsetDate = new OffsetDate(localDate, Offset.FromHours(2));
        var zonedDateTime = new ZonedDateTime(localDateTime, DateTimeZone.Utc, Offset.Zero);

        // Act

        var containsLocalDate = academicMonth.Contains(localDate);
        var containsLocalDateTime = academicMonth.Contains(localDateTime);
        var containsOffsetDate = academicMonth.Contains(offsetDate);
        var containsOffsetDateTime = academicMonth.Contains(offsetDateTime);
        var containsZonedDateTime = academicMonth.Contains(zonedDateTime);

        // Assert

        containsLocalDate.ShouldBe(expectedContains);
        containsLocalDateTime.ShouldBe(expectedContains);
        containsOffsetDate.ShouldBe(expectedContains);
        containsOffsetDateTime.ShouldBe(expectedContains);
        containsZonedDateTime.ShouldBe(expectedContains);
    }

    [Theory]
    [InlineData(2023, 8, false)]
    [InlineData(2023, 9, true)]
    [InlineData(2023, 10, false)]
    public void GivenYearMonth_WhenContainsChecked_ThenOverlapResultShouldMatch(
        int year, int month, bool expectedContains)
    {
        // Arrange

        var academicMonth = new AcademicMonth(new AcademicYear(2023), monthOfAcademicYear: 1);

        // Act

        var actual = academicMonth.Contains(new YearMonth(year, month));

        // Assert

        actual.ShouldBe(expectedContains);
    }

    [Fact]
    public void GivenMonthBeforeCommonEra_WhenNextWouldBeYearZero_ThenNoneShouldBeReturned()
    {
        // Arrange

        var academicMonth = new AcademicMonth(new AcademicYear(Era.BeforeCommon, startYearOfEra: 2), monthOfAcademicYear: 12);

        // Act

        var actual = academicMonth.GetNext();

        // Assert

        actual.IsNone.ShouldBeTrue();
    }

    [Fact]
    public void GivenMonth_WhenCompared_ThenResultShouldMatch()
    {
        // Arrange

        var firstMonth = new AcademicMonth(new AcademicYear(2023), 1);
        var secondMonth = new AcademicMonth(new AcademicYear(2023), 2);

        // Act and Assert

        (firstMonth < secondMonth).ShouldBeTrue();
        firstMonth.Equals(new AcademicMonth(new AcademicYear(2023), 1)).ShouldBeTrue();
        firstMonth.ToString().ShouldBe("2023/2024-01");
    }

    [Theory]
    [InlineData(1, "[2023-08-01, 2023-08-31]", "[2023-10-01, 2023-10-31]")]
    [InlineData(12, "[2024-07-01, 2024-07-31]", "[2024-09-01, 2024-09-30]")]
    public void GivenMonth_WhenNextAndPreviousRequested_ThenResultShouldMatch(
        int monthOfAcademicYear, string expectedPrevious, string expectedNext)
    {
        // Arrange

        var academicMonth = new AcademicMonth(new AcademicYear(2023), monthOfAcademicYear);

        // Act

        var actualPrevious = academicMonth.GetPrevious().MatchUnsafe(
            m => m.ToDateInterval().ToString(),
            () => null);
        var actualNext = academicMonth.GetNext().MatchUnsafe(
            m => m.ToDateInterval().ToString(),
            () => null);

        // Assert

        actualPrevious.ShouldBe(expectedPrevious);
        actualNext.ShouldBe(expectedNext);
    }

    [Theory]
    [InlineData(1, "[2023-09-01, 2023-09-30]")]
    [InlineData(12, "[2024-08-01, 2024-08-31]")]
    public void GivenMonth_WhenToDateInterval_ThenResultShouldMatch(int monthOfAcademicYear, string expected)
    {
        // Arrange

        var academicMonth = new AcademicMonth(new AcademicYear(2023), monthOfAcademicYear);

        // Act

        var actual = academicMonth.ToDateInterval().ToString();

        // Assert

        actual.ShouldBe(expected);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(13)]
    public void GivenInvalidMonthOrdinal_WhenCreated_ThenShouldThrow(int monthOfAcademicYear)
    {
        // Act
        Action action = () => _ = new AcademicMonth(new AcademicYear(2023), monthOfAcademicYear);

        // Assert
        action.ShouldThrow<ArgumentOutOfRangeException>();
    }
}
