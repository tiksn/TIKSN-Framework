using Newtonsoft.Json;
using System;
using System.Diagnostics.Contracts;
using TIKSN.Analytics.Telemetry;

namespace TIKSN.Serialization
{
    public class JsonSerializer : ISerializer
    {
        private readonly IExceptionTelemeter _exceptionTelemeter;

        public JsonSerializer(IExceptionTelemeter exceptionTelemeter)
        {
            Contract.Requires<ArgumentNullException>(exceptionTelemeter != null);
            _exceptionTelemeter = exceptionTelemeter;
        }

        public string Serialize(object obj)
        {
            try
            {
                return JsonConvert.SerializeObject(obj);
            }
            catch (Exception ex)
            {
                _exceptionTelemeter.TrackException(ex);

                return null;
            }
        }
    }
}