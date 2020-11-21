using Newtonsoft.Json;

namespace TIKSN.Serialization
{
    public class JsonDeserializer : DeserializerBase<string>
    {
        protected override T DeserializeInternal<T>(string serial)
        {
            return JsonConvert.DeserializeObject<T>(serial);
        }
    }
}