using Newtonsoft.Json;

namespace TIKSN.Serialization
{
    public class JsonSerializer : SerializerBase<string>
    {
        protected override string SerializeInternal(object obj)
        {
            return JsonConvert.SerializeObject(obj);
        }
    }
}