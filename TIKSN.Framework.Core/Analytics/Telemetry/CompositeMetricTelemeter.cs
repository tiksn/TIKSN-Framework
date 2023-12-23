using System.Diagnostics;
using TIKSN.Configuration;

namespace TIKSN.Analytics.Telemetry;

public class CompositeMetricTelemeter : IMetricTelemeter
{
    private readonly IPartialConfiguration<CommonTelemetryOptions> commonConfiguration;
    private readonly IEnumerable<IMetricTelemeter> metricTelemeters;

    public CompositeMetricTelemeter(
        IPartialConfiguration<CommonTelemetryOptions> commonConfiguration,
        IEnumerable<IMetricTelemeter> metricTelemeters)
    {
        this.commonConfiguration = commonConfiguration ?? throw new ArgumentNullException(nameof(commonConfiguration));
        this.metricTelemeters = metricTelemeters ?? throw new ArgumentNullException(nameof(metricTelemeters));
    }

    public async Task TrackMetricAsync(string metricName, decimal metricValue)
    {
        if (this.commonConfiguration.GetConfiguration().IsMetricTrackingEnabled)
        {
            foreach (var metricTelemeter in this.metricTelemeters)
            {
#pragma warning disable CA1031 // Do not catch general exception types
                try
                {
                    await metricTelemeter.TrackMetricAsync(metricName, metricValue).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                }
#pragma warning restore CA1031 // Do not catch general exception types
            }
        }
    }
}
