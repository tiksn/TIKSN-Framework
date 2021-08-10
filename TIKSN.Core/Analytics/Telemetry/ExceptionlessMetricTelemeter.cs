using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Exceptionless;

namespace TIKSN.Analytics.Telemetry
{
    public class ExceptionlessMetricTelemeter : ExceptionlessTelemeterBase, IMetricTelemeter
    {
        public async Task TrackMetric(string metricName, decimal metricValue)
        {
            try
            {
                ExceptionlessClient.Default.CreateFeatureUsage(metricName).SetValue(metricValue).Submit();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }
    }
}
