using Newtonsoft.Json;

namespace TIKSN.Serialization
{
    public class JsonSerializer : SerializerBase<string>
    {
        protected override string SerializeInternal<T>(T obj) => JsonConvert.SerializeObject(obj);
    }
}
