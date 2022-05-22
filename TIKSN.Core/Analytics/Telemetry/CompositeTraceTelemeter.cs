using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using TIKSN.Configuration;

namespace TIKSN.Analytics.Telemetry
{
    public class CompositeTraceTelemeter : ITraceTelemeter
    {
        private readonly IPartialConfiguration<CommonTelemetryOptions> commonConfiguration;
        private readonly IEnumerable<ITraceTelemeter> traceTelemeters;

        public CompositeTraceTelemeter(IPartialConfiguration<CommonTelemetryOptions> commonConfiguration,
            IEnumerable<ITraceTelemeter> traceTelemeters)
        {
            this.commonConfiguration = commonConfiguration;
            this.traceTelemeters = traceTelemeters;
        }

        public async Task TrackTraceAsync(string message)
        {
            if (this.commonConfiguration.GetConfiguration().IsTraceTrackingEnabled)
            {
                foreach (var traceTelemeter in this.traceTelemeters)
                {
                    try
                    {
                        await traceTelemeter.TrackTraceAsync(message).ConfigureAwait(false);
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(ex);
                    }
                }
            }
        }

        public async Task TrackTraceAsync(string message, TelemetrySeverityLevel severityLevel)
        {
            if (this.commonConfiguration.GetConfiguration().IsTraceTrackingEnabled)
            {
                foreach (var traceTelemeter in this.traceTelemeters)
                {
                    try
                    {
                        await traceTelemeter.TrackTraceAsync(message, severityLevel).ConfigureAwait(false);
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
