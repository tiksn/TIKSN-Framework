using Microsoft.ApplicationInsights.DataContracts;
using System;

namespace TIKSN.Analytics.Telemetry
{
	public class ApplicationInsightsExceptionTelemeter : IExceptionTelemeter
	{
		public void TrackException(Exception exception)
		{
			var telemetry = new ExceptionTelemetry(exception);

			ApplicationInsightsHelper.TrackException(telemetry);
		}
	}
}