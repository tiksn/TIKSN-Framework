using Newtonsoft.Json;
using TIKSN.Analytics.Telemetry;

namespace TIKSN.Serialization
{
    public class JsonDeserializer : DeserializerBase<string>
    {
        public JsonDeserializer(IExceptionTelemeter exceptionTelemeter) : base(exceptionTelemeter)
        {
        }

        protected override T DeserializeInternal<T>(string serial)
        {
            return JsonConvert.DeserializeObject<T>(serial);
        }
    }
}