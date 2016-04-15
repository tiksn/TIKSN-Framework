using Exceptionless;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace TIKSN.Analytics.Telemetry
{
	public class ExceptionlessTraceTelemeter : ExceptionlessTelemeterBase, ITraceTelemeter
	{
		public async Task TrackTrace(string message)
		{
			ExceptionlessClient.Default.CreateLog(message).Submit();
		}

		public async Task TrackTrace(string message, TelemetrySeverityLevel severityLevel)
		{
			ExceptionlessClient.Default.CreateLog(message).SetType(severityLevel.ToString()).Submit();
		}
	}
}
