using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;

namespace TIKSN.Serialization;

public sealed class SourceGeneratedJsonDeserializer : DeserializerBase<string>
{
    private readonly JsonSerializerContext context;

    public SourceGeneratedJsonDeserializer(JsonSerializerContext context)
        => this.context = context ?? throw new ArgumentNullException(nameof(context));

    protected override T DeserializeInternal<T>(string serial)
        => System.Text.Json.JsonSerializer.Deserialize(serial, this.GetTypeInfo<T>())
            ?? throw new InvalidOperationException("Deserialized result is null.");

    private JsonTypeInfo<T> GetTypeInfo<T>()
        => this.context.GetTypeInfo(typeof(T)) as JsonTypeInfo<T>
            ?? throw new InvalidOperationException(
                $"The JSON context does not contain metadata for type '{typeof(T).FullName}'.");
}
