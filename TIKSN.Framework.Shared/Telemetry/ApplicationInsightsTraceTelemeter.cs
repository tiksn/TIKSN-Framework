using Microsoft.ApplicationInsights.DataContracts;
using System.Threading.Tasks;

namespace TIKSN.Analytics.Telemetry
{
	public class ApplicationInsightsTraceTelemeter : ITraceTelemeter
	{
		public async Task TrackTrace(string message, TelemetrySeverityLevel severityLevel)
		{
			await TrackTraceInternal(message, severityLevel);
		}

		public async Task TrackTrace(string message)
		{
			await TrackTraceInternal(message, null);
		}

		private async Task TrackTraceInternal(string message, TelemetrySeverityLevel? severityLevel)
		{
			var telemetry = new TraceTelemetry(message);

			telemetry.SeverityLevel = ApplicationInsightsHelper.ConvertSeverityLevel(severityLevel);

			ApplicationInsightsHelper.TrackTrace(telemetry);
		}
	}
}