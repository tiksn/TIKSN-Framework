using NodaTime;
using TIKSN.Time;
using Xunit;

namespace TIKSN.Tests.Time;

public class CalendarYearTests
{
    [Theory]
    [InlineData(2023, 10, 14, 11, 43, 2, true)]
    [InlineData(2022, 10, 15, 10, 30, 2, false)]
    [InlineData(2024, 10, 15, 10, 30, 2, false)]
    public void GivenDateInfo_WhenContainsChecked_ThenResultShouldMatch(
        int year, int month, int day, int hour, int minute, int offsetInHours, bool expectedContains)
    {
        // Arrange

        var calendarYear2023 = new CalendarYear(2023);

        var yearMonth = new YearMonth(year, month);
        var localDate = new LocalDate(year, month, day);
        var localDateTime = new LocalDateTime(year, month, day, hour, minute);
        var offsetDateTime = new OffsetDateTime(localDateTime, Offset.FromHours(offsetInHours));
        var offsetDate = new OffsetDate(localDate, Offset.FromHours(2));
        var warsaw = DateTimeZoneProviders.Tzdb["Europe/Warsaw"];
        var zonedDateTime = new ZonedDateTime(localDateTime, warsaw, Offset.FromHours(offsetInHours));

        // Act

        var containsYearMonth = calendarYear2023.Contains(yearMonth);
        var containsLocalDate = calendarYear2023.Contains(localDate);
        var containsLocalDateTime = calendarYear2023.Contains(localDateTime);
        var containsOffsetDate = calendarYear2023.Contains(offsetDate);
        var containsOffsetDateTime = calendarYear2023.Contains(offsetDateTime);
        var containsZonedDateTime = calendarYear2023.Contains(zonedDateTime);

        // Assert

        _ = containsYearMonth.Should().Be(expectedContains);
        _ = containsLocalDate.Should().Be(expectedContains);
        _ = containsLocalDateTime.Should().Be(expectedContains);
        _ = containsOffsetDate.Should().Be(expectedContains);
        _ = containsOffsetDateTime.Should().Be(expectedContains);
        _ = containsZonedDateTime.Should().Be(expectedContains);
    }

    [Theory]
    [InlineData(2022, "2021", "2023")]
    [InlineData(2023, "2022", "2024")]
    [InlineData(2024, "2023", "2025")]
    public void GivenYear_WhenNextAndPreviousRequested_ThenResultShouldMatch(
        int year, string expectedPrevious, string expectedNext)
    {
        // Arrange

        var calendarYear = new CalendarYear(year);

        // Act

        var actualPrevious = calendarYear.GetPrevious().ToString();
        var actualNext = calendarYear.GetNext().ToString();

        // Assert

        _ = actualPrevious.Should().Be(expectedPrevious);
        _ = actualNext.Should().Be(expectedNext);
    }

    [Theory]
    [InlineData(2022, "[2022-01-01, 2022-12-31]")]
    [InlineData(2023, "[2023-01-01, 2023-12-31]")]
    [InlineData(2024, "[2024-01-01, 2024-12-31]")]
    public void GivenYear_WhenToDateInterval_ThenResultShouldMatch(
        int year, string expected)
    {
        // Arrange

        var calendarYear = new CalendarYear(year);

        // Act

        var actual = calendarYear.ToDateInterval().ToString();

        // Assert

        _ = actual.Should().Be(expected);
    }

    [Theory]
    [InlineData(2022, "2022")]
    [InlineData(2023, "2023")]
    [InlineData(2024, "2024")]
    public void GivenYear_WhenToString_ThenResultShouldMatch(
        int year, string expected)
    {
        // Arrange

        var calendarYear = new CalendarYear(year);

        // Act

        var actual = calendarYear.ToString();

        // Assert

        _ = actual.Should().Be(expected);
    }
}
