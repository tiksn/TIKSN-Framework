using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;

namespace TIKSN.Analytics.Telemetry
{
	internal static class ApplicationInsightsHelper
	{
		internal static void TrackEvent(EventTelemetry telemetry)
		{
			var client = new TelemetryClient();

			client.TrackEvent(telemetry);
		}

		internal static void TrackException(ExceptionTelemetry telemetry)
		{
			var client = new TelemetryClient();

			client.TrackException(telemetry);
		}

		internal static void TrackMetric(MetricTelemetry telemetry)
		{
			var client = new TelemetryClient();

			client.TrackMetric(telemetry);
		}

		internal static void TrackTrace(TraceTelemetry telemetry)
		{
			var client = new TelemetryClient();

			client.TrackTrace(telemetry);
		}
	}
}