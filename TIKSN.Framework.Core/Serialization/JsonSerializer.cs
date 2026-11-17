using System.Diagnostics.CodeAnalysis;
using Newtonsoft.Json;

namespace TIKSN.Serialization;

[RequiresUnreferencedCode("Json.NET uses reflection to serialize values. Use SourceGeneratedJsonSerializer for trimming-compatible JSON serialization.")]
public class JsonSerializer : SerializerBase<string>
{
    protected override string SerializeInternal<T>(T obj) => JsonConvert.SerializeObject(obj);
}
