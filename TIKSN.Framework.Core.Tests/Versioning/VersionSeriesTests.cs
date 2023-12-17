using System.Linq;
using FluentAssertions;
using NuGet.Versioning;
using TIKSN.Versioning;
using Xunit;

namespace TIKSN.Tests.Versioning;

public class VersionSeriesTests
{
    [Theory]
    [InlineData("1")]
    [InlineData("1.2")]
    [InlineData("1.2.3")]
    [InlineData("1.2.3.4")]
    [InlineData("1.2-rc")]
    [InlineData("1.2.3-rc")]
    [InlineData("1.2.3.4-rc")]
    [InlineData("1.2-rc.5")]
    [InlineData("1.2.3-rc.5")]
    [InlineData("1.2.3.4-rc.5")]
    public void GivenSeries_WhenVersionSeriesCreated_ThenParseShouldMatchToToString(string series)
    {
        var versionSeries = VersionSeries.Parse(series);

        _ = versionSeries.ToString().Should().Be(series);
    }

    [Theory]
    [InlineData("1")]
    [InlineData("1.2")]
    [InlineData("1.2.3")]
    [InlineData("1.2.3.4")]
    [InlineData("1.2-rc")]
    [InlineData("1.2.3-rc")]
    [InlineData("1.2.3.4-rc")]
    [InlineData("1.2-rc.5")]
    [InlineData("1.2.3-rc.5")]
    [InlineData("1.2.3.4-rc.5")]
    public void GivenSeries_WhenTwoIdenticalVersionSeriesCreated_ThenTheyShouldBeEqual(string series)
    {
        var versionSeries1 = VersionSeries.Parse(series);
        var versionSeries2 = VersionSeries.Parse(series);

        _ = versionSeries1.Equals(versionSeries2).Should().BeTrue();
    }

    [Theory]
    [InlineData("1", "1.2", true)]
    [InlineData("1", "1.0", true)]
    [InlineData("1", "1.2.3.4-rc.5", true)]
    [InlineData("1", "2.1", false)]
    [InlineData("1", "2.2.3.4-rc.5", false)]
    [InlineData("1.2", "1.1", false)]
    [InlineData("1.2", "1.2.3.4-rc.5", true)]
    [InlineData("1.2", "1.1.3.4-rc.5", false)]
    [InlineData("1.2.3", "1.2.3.4-rc.5", true)]
    [InlineData("1.2.3", "1.2.2.4-rc.5", false)]
    [InlineData("1.2.3", "1.2.2", false)]
    [InlineData("1.2.3", "1.2.4", false)]
    [InlineData("1.2.3.4", "1.1", false)]
    [InlineData("1.2.3.4", "1.2", false)]
    [InlineData("1.2.3.4", "1.2.3", false)]
    [InlineData("1.2.3.4", "1.2.3.4", true)]
    [InlineData("1.2.3.4", "1.2.3.4-rc", true)]
    [InlineData("1.2.3.4", "1.2.3.4-rc.5", true)]
    [InlineData("1.2-rc", "1.2", false)]
    [InlineData("1.2-rc", "1.2-rc", true)]
    [InlineData("1.2-rc", "1.2-rc.5", true)]
    [InlineData("1.2.3-rc", "1.2.3", false)]
    [InlineData("1.2.3-rc", "1.2.3-rc", true)]
    [InlineData("1.2.3-rc", "1.2.3-rc.5", true)]
    [InlineData("1.2.3-rc", "1.2.3.4-rc.5", false)]
    [InlineData("1.2.3.4-rc", "1.2.3", false)]
    [InlineData("1.2.3.4-rc", "1.2.3.4", false)]
    [InlineData("1.2.3.4-rc", "1.2.3.4-rc", true)]
    [InlineData("1.2.3.4-rc", "1.2.3.4-alpha", false)]
    [InlineData("1.2.3.4-rc", "1.2.3.4-rc.5", true)]
    [InlineData("1.2.3.4-rc", "1.2.3.4-alpha.5", false)]
    [InlineData("1.2-rc.5", "1.2-rc", false)]
    [InlineData("1.2-rc.5", "1.2-rc.5", true)]
    [InlineData("1.2-rc.5", "1.2.3-rc.5", false)]
    [InlineData("1.2-rc.5", "1.2.3.4-rc.5", false)]
    [InlineData("1.2.3-rc.5", "1.2.3", false)]
    [InlineData("1.2.3-rc.5", "1.2.3-rc", false)]
    [InlineData("1.2.3-rc.5", "1.2.3-rc.5", true)]
    [InlineData("1.2.3-rc.5", "1.2.3-rc.4", false)]
    [InlineData("1.2.3-rc.5", "1.2.3-rc.6", false)]
    [InlineData("1.2.3-rc.5", "1.2.3.4-rc.5", false)]
    [InlineData("1.2.3.4-rc.5", "1.2.3.4-rc", false)]
    [InlineData("1.2.3.4-rc.5", "1.2.3.4-rc.5", true)]
    [InlineData("1.2.3.4-rc.5", "1.2.3.4-rc.6", false)]
    [InlineData("1.2.3.4-rc.5", "1.2.3-rc.5", false)]
    public void GivenSeriesAndVersion_WhenMatchesChecked_ThenBeShouldBeAsExpected(
        string series, string version, bool matches)
    {
        // Arrange
        var versionSeries = VersionSeries.Parse(series);
        var versionToTest = (Version)NuGetVersion.Parse(version);

        // Act
        var match = versionSeries.Matches(versionToTest);

        // Assert
        _ = match.IsSome.Should().Be(matches);

        if (match.IsSome)
        {
            _ = match.Match(v => v, () => new Version(0, 0))
                .Should().Be(versionToTest);
        }
    }

    [Theory]
    [InlineData("", "")]
    [InlineData("1.2", "1.2")]
    [InlineData("1.2|1.3|1.4", "1.2")]
    [InlineData("1.2-rc.5|1.2|1.3|1.4", "1.2-rc.5|1.2")]
    [InlineData("1.3|1.3-rc|1.3-rc.5", "")]
    public void GivenVersionsList_WhenMatch_ThenResultListShouldBe(string versionsList, string matchesList)
    {
        // Arrange
        var versionSeries = VersionSeries.Parse("1.2");
        var versions = versionsList.Split('|', System.StringSplitOptions.RemoveEmptyEntries)
            .Select(x => (Version)NuGetVersion.Parse(x.Trim()))
            .ToArray();
        var expectedMatches = matchesList.Split('|', System.StringSplitOptions.RemoveEmptyEntries)
            .Select(x => (Version)NuGetVersion.Parse(x.Trim()))
            .ToArray();
        var expectedHasMatches = expectedMatches.Any();

        // Act
        var match = versionSeries.Matches(versions);

        // Assert
        _ = match.IsSome.Should().Be(expectedHasMatches);
        _ = match.Match(result => result, System.Array.Empty<Version>)
            .Should()
            .BeEquivalentTo(expectedMatches);
    }
}
