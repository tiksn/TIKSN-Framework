using Microsoft.ApplicationInsights.DataContracts;
using System;
using System.Threading.Tasks;

namespace TIKSN.Analytics.Telemetry
{
	public class ApplicationInsightsExceptionTelemeter : IExceptionTelemeter
	{
		public async Task TrackException(Exception exception)
		{
			await TrackExceptionInternal(exception, null);
		}

		public async Task TrackException(Exception exception, TelemetrySeverityLevel severityLevel)
		{
			await TrackExceptionInternal(exception, severityLevel);
		}

		private async Task TrackExceptionInternal(Exception exception, TelemetrySeverityLevel? severityLevel)
		{
			var telemetry = new ExceptionTelemetry(exception);

			telemetry.SeverityLevel = ApplicationInsightsHelper.ConvertSeverityLevel(severityLevel);

			ApplicationInsightsHelper.TrackException(telemetry);
		}
	}
}