using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using TIKSN.Configuration;

namespace TIKSN.Analytics.Telemetry
{
    public class CompositeMetricTelemeter : IMetricTelemeter
    {
        private readonly IPartialConfiguration<CommonTelemetryOptions> commonConfiguration;
        private readonly IEnumerable<IMetricTelemeter> metricTelemeters;

        public CompositeMetricTelemeter(IPartialConfiguration<CommonTelemetryOptions> commonConfiguration,
            IEnumerable<IMetricTelemeter> metricTelemeters)
        {
            this.commonConfiguration = commonConfiguration;
            this.metricTelemeters = metricTelemeters;
        }

        public async Task TrackMetric(string metricName, decimal metricValue)
        {
            if (this.commonConfiguration.GetConfiguration().IsMetricTrackingEnabled)
            {
                foreach (var metricTelemeter in this.metricTelemeters)
                {
                    try
                    {
                        await metricTelemeter.TrackMetric(metricName, metricValue);
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
