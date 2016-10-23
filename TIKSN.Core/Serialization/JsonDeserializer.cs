using Newtonsoft.Json;
using System;
using System.Diagnostics.Contracts;
using TIKSN.Analytics.Telemetry;

namespace TIKSN.Serialization
{
    public class JsonDeserializer : IDeserializer
    {
        private readonly IExceptionTelemeter _exceptionTelemeter;

        public JsonDeserializer(IExceptionTelemeter exceptionTelemeter)
        {
            Contract.Requires<ArgumentNullException>(exceptionTelemeter != null);

            _exceptionTelemeter = exceptionTelemeter;
        }

        public T Deserialize<T>(string text)
        {
            try
            {
                return JsonConvert.DeserializeObject<T>(text);
            }
            catch (Exception ex)
            {
                _exceptionTelemeter.TrackException(ex);

                return default(T);
            }
        }
    }
}