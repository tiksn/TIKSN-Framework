using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace TIKSN.Serialization;

[RequiresDynamicCode(
    "System.Text.Json may generate code at runtime when serializing values. Use SourceGeneratedJsonSerializer for NativeAOT-compatible JSON serialization.")]
[RequiresUnreferencedCode(
    "System.Text.Json uses reflection to serialize values. Use SourceGeneratedJsonSerializer for trimming-compatible JSON serialization.")]
public class JsonSerializer : SerializerBase<string>
{
    private static readonly JsonSerializerOptions SerializerOptions = new() { IncludeFields = true };

    protected override string SerializeInternal<T>(T obj)
        => System.Text.Json.JsonSerializer.Serialize(obj, SerializerOptions);
}
