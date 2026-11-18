using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;

namespace TIKSN.Serialization;

public sealed class SourceGeneratedJsonSerializer : SerializerBase<string>
{
    private readonly JsonSerializerContext context;

    public SourceGeneratedJsonSerializer(JsonSerializerContext context)
        => this.context = context ?? throw new ArgumentNullException(nameof(context));

    protected override string SerializeInternal<T>(T obj)
        => System.Text.Json.JsonSerializer.Serialize(obj, this.GetTypeInfo<T>());

    private JsonTypeInfo<T> GetTypeInfo<T>()
        => this.context.GetTypeInfo(typeof(T)) as JsonTypeInfo<T>
            ?? throw new InvalidOperationException(
                $"The JSON context does not contain metadata for type '{typeof(T).FullName}'.");
}
