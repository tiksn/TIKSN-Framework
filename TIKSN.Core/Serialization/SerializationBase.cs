using System;
using System.Diagnostics.Contracts;
using TIKSN.Analytics.Telemetry;

namespace TIKSN.Serialization
{
	public abstract class SerializationBase
	{
		protected readonly IExceptionTelemeter _exceptionTelemeter;

		public SerializationBase(IExceptionTelemeter exceptionTelemeter)
		{
			Contract.Requires<ArgumentNullException>(exceptionTelemeter != null);
			_exceptionTelemeter = exceptionTelemeter;
		}
	}
}