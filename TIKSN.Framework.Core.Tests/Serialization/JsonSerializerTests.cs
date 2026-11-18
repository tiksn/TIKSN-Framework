using System.Diagnostics.CodeAnalysis;
using Shouldly;
using TIKSN.Serialization;
using Xunit;

namespace TIKSN.Tests.Serialization;

public class JsonSerializerTests
{
    [Fact]
    [UnconditionalSuppressMessage(
        "AOT",
        "IL3050",
        Justification = "This test exercises the reflection-based serializer with statically known test models.")]
    [UnconditionalSuppressMessage(
        "Trimming",
        "IL2026",
        Justification = "This test exercises the reflection-based serializer with statically known test models.")]
    public void GivenObject_WhenSerializedAndDeserialized_ThenPreservesPropertiesAndFields()
    {
        var serializer = new JsonSerializer();
        var deserializer = new JsonDeserializer();
        var value = new SerializationModel
        {
            Id = 23,
            Name = "System.Text.Json",
            Category = "AOT"
        };

        var serialized = serializer.Serialize(value);
        var result = deserializer.Deserialize<SerializationModel>(serialized);

        result.Id.ShouldBe(23);
        result.Name.ShouldBe("System.Text.Json");
        result.Category.ShouldBe("AOT");
    }

    [Fact]
    [UnconditionalSuppressMessage(
        "AOT",
        "IL3050",
        Justification = "This test exercises the reflection-based deserializer with a statically known test model.")]
    [UnconditionalSuppressMessage(
        "Trimming",
        "IL2026",
        Justification = "This test exercises the reflection-based deserializer with a statically known test model.")]
    public void GivenPropertyNamesWithDifferentCasing_WhenDeserialized_ThenMatchesProperties()
    {
        var deserializer = new JsonDeserializer();

        var result = deserializer.Deserialize<SerializationModel>(
            /*lang=json,strict*/
                                 """{"id":23,"name":"System.Text.Json","category":"AOT"}""");

        result.Id.ShouldBe(23);
        result.Name.ShouldBe("System.Text.Json");
        result.Category.ShouldBe("AOT");
    }

    [SuppressMessage(
        "Design",
        "CA1051",
        Justification = "The public field verifies IncludeFields serialization compatibility.")]
    public sealed class SerializationModel
    {
        public string Category = string.Empty;

        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;
    }
}
