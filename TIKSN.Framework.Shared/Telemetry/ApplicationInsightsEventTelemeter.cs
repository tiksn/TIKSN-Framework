using Microsoft.ApplicationInsights.DataContracts;
using System.Threading.Tasks;

namespace TIKSN.Analytics.Telemetry
{
	public class ApplicationInsightsEventTelemeter : IEventTelemeter
	{
		public async Task TrackEvent(string name)
		{
			var telemetry = new EventTelemetry(name);

			ApplicationInsightsHelper.TrackEvent(telemetry);
		}
	}
}