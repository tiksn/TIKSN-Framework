using Microsoft.ApplicationInsights.DataContracts;

namespace TIKSN.Analytics.Telemetry
{
	public class ApplicationInsightsTraceTelemeter : ITraceTelemeter
	{
		public void TrackTrace(string message)
		{
			var telemetry = new TraceTelemetry(message);

			ApplicationInsightsHelper.TrackTrace(telemetry);
		}
	}
}
