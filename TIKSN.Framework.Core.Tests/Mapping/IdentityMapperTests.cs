using System;
using System.Collections;
using Autofac;
using Autofac.Core.Registration;
using TIKSN.DependencyInjection;
using TIKSN.Mapping;
using Xunit;

namespace TIKSN.Tests.Mapping;

public class IdentityMapperTests
{
    [Fact]
    public void GivenRegisteredContainer_WhenIdentityMapperRequested_ThenMappingShouldBeTheSame()
    {
        // Arrange

        var containerBuilder = new ContainerBuilder();
        _ = containerBuilder.RegisterModule<CoreModule>();
        var container = containerBuilder.Build();
        var expected = Guid.NewGuid();

        // Act

        var mapper = container.Resolve<IMapper<Guid, Guid>>();
        var actual = mapper.Map(expected);

        // Assert

        _ = actual.Should().Be(expected);
    }

    [Fact]
    public void GivenRegisteredContainer_WhenUnregisteredMapperRequested_ThenShouldThrow()
    {
        // Arrange

        var containerBuilder = new ContainerBuilder();
        _ = containerBuilder.RegisterModule<CoreModule>();
        var container = containerBuilder.Build();

        // Act

        Action resolveMapper = () => container.Resolve<IMapper<Version, ArrayList>>();

        // Assert

        _ = resolveMapper.Should().Throw<ComponentNotRegisteredException>();
    }
}
