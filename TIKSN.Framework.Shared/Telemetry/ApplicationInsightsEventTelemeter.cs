using Microsoft.ApplicationInsights.DataContracts;

namespace TIKSN.Analytics.Telemetry
{
	public class ApplicationInsightsEventTelemeter : IEventTelemeter
	{
		public void TrackEvent(string name)
		{
			var telemetry = new EventTelemetry(name);

			ApplicationInsightsHelper.TrackEvent(telemetry);
		}
	}
}
