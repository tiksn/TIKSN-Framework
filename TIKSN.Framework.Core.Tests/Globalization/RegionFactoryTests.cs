using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using TIKSN.DependencyInjection;
using TIKSN.Globalization;
using Xunit;

namespace TIKSN.Tests.Globalization;

public class RegionFactoryTests
{
    [Fact]
    public void GivenInvalidRegionName_WhenTryCreateCalled_ThenItShouldReturnFalse()
    {
        // Arrange

        var regionFactory = CreateRegionFactory();

        // Act

        var created = regionFactory.TryCreate("not-a-region", out var region);

        // Assert

        created.ShouldBeFalse();
        region.ShouldBeNull();
    }

    [Fact]
    public void GivenNullRegionName_WhenTryCreateCalled_ThenItShouldReturnFalse()
    {
        // Arrange

        var regionFactory = CreateRegionFactory();
        string name = null;

        // Act

        var created = regionFactory.TryCreate(name, out var region);

        // Assert

        created.ShouldBeFalse();
        region.ShouldBeNull();
    }

    [Fact]
    public void GivenValidCultureName_WhenTryCreateCalled_ThenItShouldReturnRegion()
    {
        // Arrange

        var regionFactory = CreateRegionFactory();

        // Act

        var created = regionFactory.TryCreate("en-US", out var region);

        // Assert

        created.ShouldBeTrue();
        region.ShouldNotBeNull();
        region.TwoLetterISORegionName.ShouldBe("US");
    }

    [Fact]
    public void GivenValidRegionName_WhenTryCreateCalled_ThenItShouldReturnRegion()
    {
        // Arrange

        var regionFactory = CreateRegionFactory();

        // Act

        var created = regionFactory.TryCreate("US", out var region);

        // Assert

        created.ShouldBeTrue();
        region.ShouldNotBeNull();
        region.TwoLetterISORegionName.ShouldBe("US");
    }

    private static IRegionFactory CreateRegionFactory()
    {
        var services = new ServiceCollection();
        _ = services.AddFrameworkCore();
        var serviceProvider = services.BuildServiceProvider();
        return serviceProvider.GetRequiredService<IRegionFactory>();
    }
}
