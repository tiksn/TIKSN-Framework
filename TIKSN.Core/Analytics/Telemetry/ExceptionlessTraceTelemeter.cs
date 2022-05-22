using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Exceptionless;

namespace TIKSN.Analytics.Telemetry
{
    public class ExceptionlessTraceTelemeter : ExceptionlessTelemeterBase, ITraceTelemeter
    {
        public Task TrackTraceAsync(string message)
        {
            try
            {
                ExceptionlessClient.Default.CreateLog(message).Submit();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }

            return Task.CompletedTask;
        }

        public Task TrackTraceAsync(string message, TelemetrySeverityLevel severityLevel)
        {
            try
            {
                ExceptionlessClient.Default.CreateLog(message).SetType(severityLevel.ToString()).Submit();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }

            return Task.CompletedTask;
        }
    }
}
