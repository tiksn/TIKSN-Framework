using LiteGuard;
using TIKSN.Analytics.Telemetry;

namespace TIKSN.Serialization
{
	public abstract class SerializationBase
	{
		protected readonly IExceptionTelemeter _exceptionTelemeter;

		public SerializationBase(IExceptionTelemeter exceptionTelemeter)
		{
			Guard.AgainstNullArgument(nameof(exceptionTelemeter), exceptionTelemeter);

			_exceptionTelemeter = exceptionTelemeter;
		}
	}
}