using System.Management.Automation;
using System.Threading.Tasks;

namespace TIKSN.Analytics.Telemetry
{
    public class PowerShellMetricTelemeter : IMetricTelemeter
    {
        private readonly Cmdlet cmdlet;

        public PowerShellMetricTelemeter(Cmdlet cmdlet) => this.cmdlet = cmdlet;

        public Task TrackMetric(string metricName, decimal metricValue)
        {
            this.cmdlet.WriteVerbose($"METRIC: {metricName} - {metricValue}");

            return Task.FromResult<object>(null);
        }
    }
}
