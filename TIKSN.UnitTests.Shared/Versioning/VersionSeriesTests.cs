using FluentAssertions;
using Xunit;

namespace TIKSN.Versioning.Tests
{
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

            versionSeries.ToString().Should().Be(series);
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

            versionSeries1.Equals(versionSeries2).Should().BeTrue();
        }
    }
}
