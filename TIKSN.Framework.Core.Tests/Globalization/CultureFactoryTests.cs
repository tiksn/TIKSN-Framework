using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using TIKSN.DependencyInjection;
using TIKSN.Globalization;
using Xunit;

namespace TIKSN.Tests.Globalization;

public class CultureFactoryTests
{
    [Fact]
    public void GivenInvalidCultureName_WhenTryCreateCalled_ThenItShouldReturnFalse()
    {
        // Arrange

        var cultureFactory = CreateCultureFactory();

        // Act

        var created = cultureFactory.TryCreate("this is not valid", out var culture);

        // Assert

        created.ShouldBeFalse();
        culture.ShouldBeNull();
    }

    [Fact]
    public void GivenNullCultureName_WhenTryCreateCalled_ThenItShouldReturnFalse()
    {
        // Arrange

        var cultureFactory = CreateCultureFactory();
        string name = null;

        // Act

        var created = cultureFactory.TryCreate(name, out var culture);

        // Assert

        created.ShouldBeFalse();
        culture.ShouldBeNull();
    }

    [Fact]
    public void GivenValidCultureName_WhenTryCreateCalled_ThenItShouldReturnCulture()
    {
        // Arrange

        var cultureFactory = CreateCultureFactory();

        // Act

        var created = cultureFactory.TryCreate("en-US", out var culture);

        // Assert

        created.ShouldBeTrue();
        culture.ShouldNotBeNull();
        culture.Name.ShouldBe("en-US");
    }

    private static ICultureFactory CreateCultureFactory()
    {
        var services = new ServiceCollection();
        _ = services.AddFrameworkCore();
        var serviceProvider = services.BuildServiceProvider();
        return serviceProvider.GetRequiredService<ICultureFactory>();
    }
}
