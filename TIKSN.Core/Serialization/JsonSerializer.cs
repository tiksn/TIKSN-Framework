using Newtonsoft.Json;

namespace TIKSN.Serialization
{
    public class JsonSerializer : SerializerBase<string>
    {
        protected override string SerializeInternal<T>(T obj)
        {
            return JsonConvert.SerializeObject(obj);
        }
    }
}