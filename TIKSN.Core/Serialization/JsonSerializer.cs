using Newtonsoft.Json;
using TIKSN.Analytics.Telemetry;

namespace TIKSN.Serialization
{
    public class JsonSerializer : SerializerBase
    {
        public JsonSerializer(IExceptionTelemeter exceptionTelemeter) : base(exceptionTelemeter)
        {
        }

        protected override string SerializeInternal(object obj)
        {
            return JsonConvert.SerializeObject(obj);
        }
    }
}