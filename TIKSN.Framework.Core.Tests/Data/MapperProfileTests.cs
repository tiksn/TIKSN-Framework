using System;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using TIKSN.DependencyInjection;
using TIKSN.Mapping;
using Xunit;

namespace TIKSN.Data.Tests
{
    public class MapperProfileTests
    {
        private readonly IServiceProvider serviceProvider;

        public MapperProfileTests()
        {
            var services = new ServiceCollection();
            _ = services.AddFrameworkPlatform(new[] { new SampleMapperProfile(services) });

            this.serviceProvider = services.BuildServiceProvider();
        }

        [Fact]
        public void GivenRegisteredMapperProfile_WhenMapped_ThenPropertiesMatch()
        {
            // Arrange
            var source = new SampleSource
            {
                Property1 = 43,
                Property2 = "Yes",
                Property3 = Guid.Parse("6bcfa102-9513-47fd-a6b1-21a0939c911f")
            };

            var mapper = this.serviceProvider.GetRequiredService<IMapper<SampleSource, SampleDestination>>();

            // Act
            var destination = mapper.Map(source);

            // Assert
            destination.Should().NotBeNull();
            destination.Property1.Should().Be(source.Property1);
            destination.Property2.Should().Be(source.Property2);
            destination.Property3.Should().Be(source.Property3);
        }

        [Fact]
        public void GivenRegisteredMapperProfile_WhenMapperResolved_ThenResultShouldBeSuccessful()
        {
            // Act
            var mapper = this.serviceProvider.GetRequiredService<IMapper<SampleSource, SampleDestination>>();

            // Assert
            mapper.Should().NotBeNull();
        }

        public class SampleDestination
        {
            public int Property1 { get; set; }

            public string Property2 { get; set; }

            public Guid Property3 { get; set; }
        }

        public class SampleMapperProfile : MapperProfile
        {
            public SampleMapperProfile(IServiceCollection services) : base(services)
            {
                AddMapper<SampleSource, SampleDestination>();
            }
        }

        public class SampleSource
        {
            public int Property1 { get; set; }

            public string Property2 { get; set; }

            public Guid Property3 { get; set; }
        }
    }
}
