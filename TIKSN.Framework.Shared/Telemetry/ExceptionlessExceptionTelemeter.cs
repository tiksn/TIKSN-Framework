using Exceptionless;
using System;
using System.Threading.Tasks;

namespace TIKSN.Analytics.Telemetry
{
	public class ExceptionlessExceptionTelemeter : ExceptionlessTelemeterBase, IExceptionTelemeter
	{
		public ExceptionlessExceptionTelemeter()
		{

		}

		public async Task TrackException(Exception exception)
		{
			exception.ToExceptionless().Submit();
		}

		public async Task TrackException(Exception exception, TelemetrySeverityLevel severityLevel)
		{
			exception.ToExceptionless().SetType(severityLevel.ToString()).Submit();
		}
	}
}
