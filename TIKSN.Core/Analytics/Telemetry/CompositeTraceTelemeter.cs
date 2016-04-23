using System.Threading.Tasks;
using TIKSN.Configuration;

namespace TIKSN.Analytics.Telemetry
{
	public class CompositeTraceTelemeter : ITraceTelemeter
	{
		private readonly IConfiguration<CommonConfiguration> commonConfiguration;
		private readonly ITraceTelemeter[] traceTelemeters;

		public CompositeTraceTelemeter(IConfiguration<CommonConfiguration> commonConfiguration, ITraceTelemeter[] traceTelemeters)
		{
			this.commonConfiguration = commonConfiguration;
			this.traceTelemeters = traceTelemeters;
		}

		public async Task TrackTrace(string message)
		{
			if (commonConfiguration.GetConfiguration().IsTraceTrackingEnabled)
			{
				foreach (var traceTelemeter in traceTelemeters)
				{
					await traceTelemeter.TrackTrace(message);
				}
			}
		}

		public async Task TrackTrace(string message, TelemetrySeverityLevel severityLevel)
		{
			if (commonConfiguration.GetConfiguration().IsTraceTrackingEnabled)
			{
				foreach (var traceTelemeter in traceTelemeters)
				{
					await traceTelemeter.TrackTrace(message, severityLevel);
				}
			}
		}
	}
}