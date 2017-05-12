using HockeyApp.Android.Metrics;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace TIKSN.Analytics.Telemetry
{
	public class HockeyAppEventTelemeter : HockeyAppTelemeterBase, IEventTelemeter
	{
		public Task TrackEvent(string name)
		{
			MetricsManager.TrackEvent(name);

			return Task.FromResult<object>(null);
		}

		public Task TrackEvent(string name, IDictionary<string, string> properties)
		{
			MetricsManager.TrackEvent(name, properties);

			return Task.FromResult<object>(null);
		}
	}
}