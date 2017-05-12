using Microsoft.HockeyApp;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace TIKSN.Analytics.Telemetry
{
	public class HockeyAppEventTelemeter : HockeyAppTelemeterBase, IEventTelemeter
	{
		public HockeyAppEventTelemeter(IHockeyClient hockeyClient) : base(hockeyClient)
		{
		}

		public Task TrackEvent(string name)
		{
			hockeyClient.TrackEvent(name);

			return Task.FromResult<object>(null);
		}

		public Task TrackEvent(string name, IDictionary<string, string> properties)
		{
			hockeyClient.TrackEvent(name, properties);

			return Task.FromResult<object>(null);
		}
	}
}