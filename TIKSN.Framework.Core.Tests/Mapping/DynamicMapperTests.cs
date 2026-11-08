using System;
using System.Threading;
using System.Threading.Tasks;
using Shouldly;
using TIKSN.Mapping;
using Xunit;

namespace TIKSN.Tests.Mapping;

public class DynamicMapperTests
{
    [Fact]
    public async Task GivenMatchingProperties_WhenMappedAsync_ThenDestinationPropertiesAreSet()
    {
        var mapper = new DynamicMapper<SourceModel, DestinationModel>();
        var source = new SourceModel
        {
            Id = 100,
            Name = "Async Name",
            Active = false,
            Amount = 50.0m
        };

        var result = await mapper.MapAsync(source, CancellationToken.None);

        _ = result.ShouldNotBeNull();
        result.Id.ShouldBe(100);
        result.Name.ShouldBe("Async Name");
        result.Active.ShouldBe(false);
        result.Amount.ShouldBe(50.0m);
    }

    [Fact]
    public void GivenMatchingProperties_WhenMapped_ThenDestinationPropertiesAreSet()
    {
        var mapper = new DynamicMapper<SourceModel, DestinationModel>();
        var source = new SourceModel
        {
            Id = 42,
            Name = "Sample Name",
            Active = true,
            Amount = 19.99m,
            UnmatchedSourceProperty = "Ignored"
        };

        var result = mapper.Map(source);

        _ = result.ShouldNotBeNull();
        result.Id.ShouldBe(42);
        result.Name.ShouldBe("Sample Name");
        result.Active.ShouldBe(true);
        result.Amount.ShouldBe(19.99m);
        result.UnmatchedDestinationProperty.ShouldBeNull();
    }

    [Fact]
    public void GivenNullSource_WhenMapped_ThenThrowsArgumentNullException()
    {
        var mapper = new DynamicMapper<SourceModel, DestinationModel>();

        Action act = () => mapper.Map(null!);

        _ = act.ShouldThrow<ArgumentNullException>();
    }

    [Fact]
    public void GivenTypeWithoutParameterlessConstructor_WhenInstantiatedAndMapped_ThenThrowsInvalidOperationException()
    {
        var mapper = new DynamicMapper<SourceModel, NoDefaultConstructorModel>();
        var source = new SourceModel
        {
            Id = 1,
            Name = "Test"
        };

        Action act = () => mapper.Map(source);

        _ = act.ShouldThrow<InvalidOperationException>();
    }

    public class DestinationModel
    {
        public bool Active { get; set; }
        public decimal Amount { get; set; }
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? UnmatchedDestinationProperty { get; set; }
    }

    public class NoDefaultConstructorModel
    {
        public NoDefaultConstructorModel(int id) => this.Id = id;

        public int Id { get; }
    }

    public class SourceModel
    {
        public bool Active { get; set; }
        public decimal Amount { get; set; }
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string UnmatchedSourceProperty { get; set; } = string.Empty;
    }
}
