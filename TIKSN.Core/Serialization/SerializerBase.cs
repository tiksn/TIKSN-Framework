using System;
using TIKSN.Analytics.Telemetry;

namespace TIKSN.Serialization
{
    public abstract class SerializerBase<TSerial> : SerializationBase, ISerializer<TSerial> where TSerial : class
    {
        protected SerializerBase(IExceptionTelemeter exceptionTelemeter) : base(exceptionTelemeter)
        {
        }

        public TSerial Serialize(object obj)
        {
            if (obj == null)
                return null;

            try
            {
                return SerializeInternal(obj);
            }
            catch (Exception ex)
            {
                _exceptionTelemeter.TrackException(ex);

                return null;
            }
        }

        protected abstract TSerial SerializeInternal(object obj);
    }
}