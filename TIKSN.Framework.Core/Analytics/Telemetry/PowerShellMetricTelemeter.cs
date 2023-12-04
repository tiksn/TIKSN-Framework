using System.Management.Automation;

namespace TIKSN.Analytics.Telemetry;

public class PowerShellMetricTelemeter : IMetricTelemeter
{
    private readonly Cmdlet cmdlet;

    public PowerShellMetricTelemeter(Cmdlet cmdlet) => this.cmdlet = cmdlet;

    public Task TrackMetricAsync(string metricName, decimal metricValue)
    {
        this.cmdlet.WriteVerbose($"METRIC: {metricName} - {metricValue}");

        return Task.FromResult<object>(null);
    }
}
