using System.Diagnostics.CodeAnalysis;
using Newtonsoft.Json;

namespace TIKSN.Serialization;

[RequiresUnreferencedCode("Json.NET uses reflection to deserialize values. Use SourceGeneratedJsonDeserializer for trimming-compatible JSON deserialization.")]
public class JsonDeserializer : DeserializerBase<string>
{
    protected override T DeserializeInternal<T>(string serial)
        => JsonConvert.DeserializeObject<T>(serial)
            ?? throw new InvalidOperationException("Deserialized result is null.");
}
