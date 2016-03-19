using TIKSN.Configuration;

namespace TIKSN.Analytics.Telemetry
{
	public class CompositeTraceTelemeter : ITraceTelemeter
	{
		private ICommonConfiguration commonConfiguration;
		private ITraceTelemeter[] traceTelemeters;

		public CompositeTraceTelemeter(ICommonConfiguration commonConfiguration, ITraceTelemeter[] traceTelemeters)
		{
			this.commonConfiguration = commonConfiguration;
			this.traceTelemeters = traceTelemeters;
		}

		public void TrackTrace(string message)
		{
			if (commonConfiguration.IsTraceTrackingEnabled)
			{
				foreach (var traceTelemeter in traceTelemeters)
				{
					traceTelemeter.TrackTrace(message);
				}
			}
		}
	}
}