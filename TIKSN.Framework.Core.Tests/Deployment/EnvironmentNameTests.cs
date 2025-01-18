using System.Globalization;
using TIKSN.Deployment;
using Xunit;

namespace TIKSN.Tests.Deployment;

public class EnvironmentNameTests
{
    [Theory]
    [InlineData("Development", "T", "Development")]
    [InlineData("Development", "t", "Development")]
    [InlineData("Development", "u", "DEVELOPMENT")]
    [InlineData("Development", "L", "development")]
    [InlineData("Development", "l", "development")]
    [InlineData("local-development", "T", "Local-Development")]
    [InlineData("local-development", "t", "Local-Development")]
    [InlineData("local-development", "U", "LOCAL-DEVELOPMENT")]
    [InlineData("local-development", "u", "LOCAL-DEVELOPMENT")]
    [InlineData("local-development", "L", "local-development")]
    [InlineData("local-development", "l", "local-development")]
    [InlineData("CENTRAL_DEVELOPMENT", "T", "Central-Development")]
    [InlineData("CENTRAL_DEVELOPMENT", "t", "Central-Development")]
    [InlineData("CENTRAL_DEVELOPMENT", "U", "CENTRAL-DEVELOPMENT")]
    [InlineData("CENTRAL_DEVELOPMENT", "u", "CENTRAL-DEVELOPMENT")]
    [InlineData("CENTRAL_DEVELOPMENT", "L", "central-development")]
    [InlineData("CENTRAL_DEVELOPMENT", "l", "central-development")]
    public void GivenName_WhenFormatted_ThenStringMustMatch(string name, string format, string actual)
    {
        // Arrange

        // Act
        var result1 = EnvironmentName
            .Parse(name, asciiOnly: true, CultureInfo.InvariantCulture)
            .Match(s => s.ToString(format, CultureInfo.InvariantCulture), string.Empty);

        var result2 = EnvironmentName
            .Parse(name, CultureInfo.InvariantCulture)
            .ToString(format, CultureInfo.InvariantCulture);

        // Assert
        _ = result1.Should().Be(actual);
        _ = result2.Should().Be(actual);
    }

    [Theory]
    [InlineData("", "")]
    [InlineData("Development", "Development")]
    [InlineData("Test", "Test")]
    [InlineData("Staging", "Staging")]
    [InlineData("Production", "Production")]
    [InlineData("Local-Development", "Local-Development")]
    [InlineData("Mike-Local-Development", "Mike-Local-Development")]
    [InlineData("Central-Development", "Central-Development")]
    [InlineData("Central_Development", "Central-Development")]
    [InlineData("Development<>", "")]
    [InlineData("<>Development<>", "")]
    [InlineData("<>Development", "")]
    [InlineData("Central_Development<>", "")]
    public void GivenName_WhenParsed_ThenIfParsableStringMustMatch(string name, string actual)
    {
        // Arrange

        // Act
        var result1 = EnvironmentName
            .Parse(name, asciiOnly: true, CultureInfo.InvariantCulture);
        var result1Formatted = result1
            .Match(s => s.ToString(), string.Empty);
        var canParse = EnvironmentName.TryParse(name, CultureInfo.InvariantCulture, out var result2);

        // Assert
        _ = result1Formatted.Should().Be(actual);
        _ = canParse.Should().Be(result1.IsSome);
        _ = result1.IfSome(x => x.Should().Be(result2));
    }

    [Theory]
    [InlineData("Development", "Development", true)]
    [InlineData("development", "Development", true)]
    [InlineData("Development", "development", true)]
    [InlineData("Development", "DEVELOPMENT", true)]
    [InlineData("DEVELOPMENT", "Development", true)]
    [InlineData("Development", "Test", false)]
    [InlineData("Staging", "Production", false)]
    [InlineData("Local-Development", "Local_Development", true)]
    [InlineData("Local+Development", "Local-Development", true)]
    [InlineData("Mike-Local-Development", "Local-Development", false)]
    [InlineData("Local-Development", "Mike-Local-Development", false)]
    [InlineData("Mike-Local-development", "Mike-LOCAL-Development", true)]
    [InlineData("Central-Development", "Local-Development", false)]
    public void GivenNames_WhenEquals_ThenMustMatch(string name1, string name2, bool isEqualExpected)
    {
        // Arrange

        var environmentName1 = EnvironmentName
            .Parse(name1, asciiOnly: true, CultureInfo.InvariantCulture)
            .Match(s => s, () => null);

        var environmentName2 = EnvironmentName
            .Parse(name2, asciiOnly: true, CultureInfo.InvariantCulture)
            .Match(s => s, () => null);

        // Act

        var isEqualActual = environmentName1.Equals(environmentName2);

        // Assert
        _ = isEqualActual.Should().Be(isEqualExpected);
    }

    [Theory]
    [InlineData("Development", "Development", true)]
    [InlineData("development", "Development", true)]
    [InlineData("Development", "development", true)]
    [InlineData("Development", "DEVELOPMENT", true)]
    [InlineData("DEVELOPMENT", "Development", true)]
    [InlineData("Development", "Test", false)]
    [InlineData("Staging", "Production", false)]
    [InlineData("Local-Development", "Local_Development", true)]
    [InlineData("Local+Development", "Local-Development", true)]
    [InlineData("Mike-Local-Development", "Local-Development", true)]
    [InlineData("Mike-Local-Development", "Development", true)]
    [InlineData("Local-Development", "Mike-Local-Development", false)]
    [InlineData("Development", "Mike-Local-Development", false)]
    [InlineData("Mike-Local-development", "Mike-LOCAL-Development", true)]
    [InlineData("Central-Development", "Local-Development", false)]
    [InlineData("Central-Development", "Development", true)]
    [InlineData("Central-Development", "Production", false)]
    public void GivenNames_WhenMatches_ThenMustMatch(string name1, string name2, bool isEqualExpected)
    {
        // Arrange

        var environmentName1 = EnvironmentName
            .Parse(name1, asciiOnly: true, CultureInfo.InvariantCulture)
            .Match(s => s, () => null);

        var environmentName2 = EnvironmentName
            .Parse(name2, asciiOnly: true, CultureInfo.InvariantCulture)
            .Match(s => s, () => null);

        // Act

        var isEqualActual = environmentName1.Matches(environmentName2);

        // Assert
        _ = isEqualActual.Should().Be(isEqualExpected);
    }
}
