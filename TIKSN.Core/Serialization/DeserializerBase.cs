using System;
using TIKSN.Analytics.Telemetry;

namespace TIKSN.Serialization
{
    public abstract class DeserializerBase : SerializationBase, IDeserializer
    {
        public DeserializerBase(IExceptionTelemeter exceptionTelemeter) : base(exceptionTelemeter)
        {
        }

        public T Deserialize<T>(string text)
        {
            try
            {
                return DeserializeInternal<T>(text);
            }
            catch (Exception ex)
            {
                _exceptionTelemeter.TrackException(ex);

                return default(T);
            }
        }

        protected abstract T DeserializeInternal<T>(string text);
    }
}
