using Microsoft.Extensions.Hosting;
using NSubstitute;
using Shouldly;
using TIKSN.Deployment;
using Xunit;

namespace TIKSN.Tests.Deployment;

public class HostEnvironmentExtensionsTests
{
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
    public void GivenNames_WhenMatches_ThenMustMatch(string hostEnvironmentName, string licenseEnvironmentName, bool matchesExpected)
    {
        // Arrange
        var hostEnvironment = Substitute.For<IHostEnvironment>();
        _ = hostEnvironment.EnvironmentName.Returns(hostEnvironmentName);

        // Act

        var matchesActual = hostEnvironment.MatchesEnvironment(licenseEnvironmentName);

        // Assert
        matchesActual.ShouldBe(matchesExpected);
    }
}
