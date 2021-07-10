using System.Threading.Tasks;

namespace TIKSN.Analytics.Telemetry
{
    public interface ITraceTelemeter
    {
        Task TrackTrace(string message);

        Task TrackTrace(string message, TelemetrySeverityLevel severityLevel);
    }
}
