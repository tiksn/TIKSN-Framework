using System;
using System.Collections.Generic;
using TIKSN.Analytics.Telemetry;
using TIKSN.Configuration;

namespace TIKSN.Analytics.Telemetry
{
	public class PushalotTraceTelemeter : PushalotTelemeterBase, ITraceTelemeter
	{
		public PushalotTraceTelemeter(IPushalotConfiguration pushalotConfiguration)
			:base(pushalotConfiguration)
		{

		}

		public void TrackTrace(string message)
		{
			SendMessage("Trace", message);
		}

		protected override IEnumerable<string> GetAuthorizationTokens(IPushalotConfiguration pushalotConfiguration)
		{
			return pushalotConfiguration.TraceAuthorizationTokens;
		}
	}
}
