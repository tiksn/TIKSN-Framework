using TIKSN.Analytics.Telemetry;

namespace TIKSN.Serialization
{
    public abstract class SerializationBase
    {
        protected readonly IExceptionTelemeter _exceptionTelemeter;

        protected SerializationBase(IExceptionTelemeter exceptionTelemeter)
        {
            _exceptionTelemeter = exceptionTelemeter ?? throw new System.ArgumentNullException(nameof(exceptionTelemeter));
        }
    }
}