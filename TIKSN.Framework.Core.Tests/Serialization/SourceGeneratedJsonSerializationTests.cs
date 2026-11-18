using System;
using System.Text.Json.Serialization;
using Shouldly;
using TIKSN.Serialization;
using Xunit;

namespace TIKSN.Tests.Serialization;

[JsonSerializable(typeof(SerializationModel))]
[JsonSerializable(typeof(string))]
internal sealed partial class TestJsonContext : JsonSerializerContext
{
}

internal sealed record SerializationModel(int Id, string Name);

internal sealed record UnregisteredModel(int Id);

public class SourceGeneratedJsonSerializationTests
{
    [Fact]
    public void GivenRegisteredType_WhenSerializedAndDeserialized_ThenPreservesValues()
    {
        var serializer = new SourceGeneratedJsonSerializer(TestJsonContext.Default);
        var deserializer = new SourceGeneratedJsonDeserializer(TestJsonContext.Default);
        var value = new SerializationModel(23, "AOT");

        var serialized = serializer.Serialize(value);
        var result = deserializer.Deserialize<SerializationModel>(serialized);

        result.ShouldBe(value);
    }

    [Fact]
    public void GivenUnregisteredType_WhenDeserialized_ThenReportsMissingMetadata()
    {
        var deserializer = new SourceGeneratedJsonDeserializer(TestJsonContext.Default);

        Action act = () => deserializer.Deserialize<UnregisteredModel>("{}");

        var exception = act.ShouldThrow<DeserializerException>();
        exception.InnerException.ShouldBeOfType<InvalidOperationException>()
            .Message.ShouldContain(typeof(UnregisteredModel).FullName!);
    }

    [Fact]
    public void GivenUnregisteredType_WhenSerialized_ThenReportsMissingMetadata()
    {
        var serializer = new SourceGeneratedJsonSerializer(TestJsonContext.Default);

        Action act = () => serializer.Serialize(new UnregisteredModel(1));

        var exception = act.ShouldThrow<SerializerException>();
        exception.InnerException.ShouldBeOfType<InvalidOperationException>()
            .Message.ShouldContain(typeof(UnregisteredModel).FullName!);
    }
}
