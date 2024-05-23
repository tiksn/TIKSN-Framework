using Newtonsoft.Json;

namespace TIKSN.Serialization;

public class JsonDeserializer : DeserializerBase<string>
{
    protected override T DeserializeInternal<T>(string serial)
        => JsonConvert.DeserializeObject<T>(serial)
            ?? throw new InvalidOperationException("Deserialized result is null.");
}
