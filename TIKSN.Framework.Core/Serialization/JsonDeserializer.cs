using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace TIKSN.Serialization;

[RequiresDynamicCode("System.Text.Json may generate code at runtime when deserializing values. Use SourceGeneratedJsonDeserializer for NativeAOT-compatible JSON deserialization.")]
[RequiresUnreferencedCode("System.Text.Json uses reflection to deserialize values. Use SourceGeneratedJsonDeserializer for trimming-compatible JSON deserialization.")]
public class JsonDeserializer : DeserializerBase<string>
{
    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        IncludeFields = true,
        PropertyNameCaseInsensitive = true
    };

    protected override T DeserializeInternal<T>(string serial)
        => global::System.Text.Json.JsonSerializer.Deserialize<T>(serial, SerializerOptions)
            ?? throw new InvalidOperationException("Deserialized result is null.");
}
