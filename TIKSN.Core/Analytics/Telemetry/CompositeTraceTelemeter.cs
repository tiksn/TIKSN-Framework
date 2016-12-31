using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using TIKSN.Configuration;

namespace TIKSN.Analytics.Telemetry
{
	public class CompositeTraceTelemeter : ITraceTelemeter
	{
		private readonly IConfiguration<CommonConfiguration> commonConfiguration;
		private readonly IEnumerable<ITraceTelemeter> traceTelemeters;

		public CompositeTraceTelemeter(IConfiguration<CommonConfiguration> commonConfiguration, IEnumerable<ITraceTelemeter> traceTelemeters)
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
					try
					{
						await traceTelemeter.TrackTrace(message);
					}
					catch (Exception ex)
					{
						Debug.WriteLine(ex);
					}
				}
			}
		}

		public async Task TrackTrace(string message, TelemetrySeverityLevel severityLevel)
		{
			if (commonConfiguration.GetConfiguration().IsTraceTrackingEnabled)
			{
				foreach (var traceTelemeter in traceTelemeters)
				{
					try
					{
						await traceTelemeter.TrackTrace(message, severityLevel);
					}
					catch (Exception ex)
					{
						Debug.WriteLine(ex);
					}
				}
			}
		}
	}
}