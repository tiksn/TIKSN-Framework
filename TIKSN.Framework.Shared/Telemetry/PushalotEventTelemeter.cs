using System.Collections.Generic;
using TIKSN.Configuration;

namespace TIKSN.Analytics.Telemetry
{
	public class PushalotEventTelemeter : PushalotTelemeterBase, IEventTelemeter
	{
		public PushalotEventTelemeter(IPushalotConfiguration pushalotConfiguration)
			: base(pushalotConfiguration)
		{
		}

		public void TrackEvent(string name)
		{
			SendMessage("Event", name);
		}

		protected override IEnumerable<string> GetAuthorizationTokens(IPushalotConfiguration pushalotConfiguration)
		{
			return pushalotConfiguration.EventAuthorizationTokens;
		}
	}
}