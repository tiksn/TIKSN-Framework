using System.Diagnostics;
using Microsoft.Extensions.Options;

namespace TIKSN.Analytics.Telemetry;

public class CompositeMetricTelemeter : IMetricTelemeter
{
    private readonly IOptions<CommonTelemetryOptions> commonOptions;
    private readonly IEnumerable<IMetricTelemeter> metricTelemeters;

    public CompositeMetricTelemeter(
        IOptions<CommonTelemetryOptions> commonOptions,
        IEnumerable<IMetricTelemeter> metricTelemeters)
    {
        this.commonOptions = commonOptions ?? throw new ArgumentNullException(nameof(commonOptions));
        this.metricTelemeters = metricTelemeters ?? throw new ArgumentNullException(nameof(metricTelemeters));
    }

    public async Task TrackMetricAsync(string metricName, decimal metricValue)
    {
        if (this.commonOptions.Value.IsMetricTrackingEnabled)
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
