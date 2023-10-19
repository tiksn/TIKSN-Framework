using FluentAssertions;
using NodaTime;
using Xunit;

namespace TIKSN.Time.Tests;

public class FiscalYearTests
{
    [Theory]
    [InlineData(2023, 10, 14, 11, 43, 2, true)]
    [InlineData(2022, 10, 15, 10, 30, 2, false)]
    [InlineData(2024, 10, 15, 10, 30, 2, false)]
    public void GivenDateInfo_WhenContainsChecked_ThenResultShouldMatch(
        int year, int month, int day, int hour, int minute, int offsetInHours, bool expectedContains)
    {
        // Arrange

        var fiscalYear2023 = new FiscalYear(2023, new AnnualDate(10, 1));

        var yearMonth = new YearMonth(year, month);
        var localDate = new LocalDate(year, month, day);
        var localDateTime = new LocalDateTime(year, month, day, hour, minute);
        var offsetDateTime = new OffsetDateTime(localDateTime, Offset.FromHours(offsetInHours));
        var offsetDate = new OffsetDate(localDate, Offset.FromHours(2));
        var warsaw = DateTimeZoneProviders.Tzdb["Europe/Warsaw"];
        var zonedDateTime = new ZonedDateTime(localDateTime, warsaw, Offset.FromHours(offsetInHours));

        // Act

        var containsYearMonth = fiscalYear2023.Contains(yearMonth);
        var containsLocalDate = fiscalYear2023.Contains(localDate);
        var containsLocalDateTime = fiscalYear2023.Contains(localDateTime);
        var containsOffsetDate = fiscalYear2023.Contains(offsetDate);
        var containsOffsetDateTime = fiscalYear2023.Contains(offsetDateTime);
        var containsZonedDateTime = fiscalYear2023.Contains(zonedDateTime);

        // Assert

        _ = containsYearMonth.Should().Be(expectedContains);
        _ = containsLocalDate.Should().Be(expectedContains);
        _ = containsLocalDateTime.Should().Be(expectedContains);
        _ = containsOffsetDate.Should().Be(expectedContains);
        _ = containsOffsetDateTime.Should().Be(expectedContains);
        _ = containsZonedDateTime.Should().Be(expectedContains);
    }

    [Theory]
    [InlineData(2022, "2021/2022", "2023/2024")]
    [InlineData(2023, "2022/2023", "2024/2025")]
    [InlineData(2024, "2023/2024", "2025/2026")]
    public void GivenYear_WhenNextAndPreviousRequested_ThenResultShouldMatch(
        int year, string expectedPrevious, string expectedNext)
    {
        // Arrange

        var fiscalYear = new FiscalYear(year, new AnnualDate(10, 1));

        // Act

        var actualPrevious = fiscalYear.GetPrevious().ToString();
        var actualNext = fiscalYear.GetNext().ToString();

        // Assert

        _ = actualPrevious.Should().Be(expectedPrevious);
        _ = actualNext.Should().Be(expectedNext);
    }

    [Theory]
    [InlineData(2022, "[2022-10-01, 2023-09-30]")]
    [InlineData(2023, "[2023-10-01, 2024-09-30]")]
    [InlineData(2024, "[2024-10-01, 2025-09-30]")]
    public void GivenYear_WhenToDateInterval_ThenResultShouldMatch(
        int year, string expected)
    {
        // Arrange

        var fiscalYear = new FiscalYear(year, new AnnualDate(10, 1));

        // Act

        var actual = fiscalYear.ToDateInterval().ToString();

        // Assert

        _ = actual.Should().Be(expected);
    }

    [Theory]
    [InlineData(2022, "2022/2023")]
    [InlineData(2023, "2023/2024")]
    [InlineData(2024, "2024/2025")]
    public void GivenYear_WhenToString_ThenResultShouldMatch(
        int year, string expected)
    {
        // Arrange

        var fiscalYear = new FiscalYear(year, new AnnualDate(10, 1));

        // Act

        var actual = fiscalYear.ToString();

        // Assert

        _ = actual.Should().Be(expected);
    }
}
