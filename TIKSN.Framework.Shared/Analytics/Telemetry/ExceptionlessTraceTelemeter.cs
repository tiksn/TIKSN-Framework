using Exceptionless;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace TIKSN.Analytics.Telemetry
{
	public class ExceptionlessTraceTelemeter : ExceptionlessTelemeterBase, ITraceTelemeter
	{
		public async Task TrackTrace(string message)
		{
			try
			{
				ExceptionlessClient.Default.CreateLog(message).Submit();
			}
			catch (Exception ex)
			{
				Debug.WriteLine(ex);
			}
		}

		public async Task TrackTrace(string message, TelemetrySeverityLevel severityLevel)
		{
			try
			{
				ExceptionlessClient.Default.CreateLog(message).SetType(severityLevel.ToString()).Submit();
			}
			catch (Exception ex)
			{
				Debug.WriteLine(ex);
			}
		}
	}
}