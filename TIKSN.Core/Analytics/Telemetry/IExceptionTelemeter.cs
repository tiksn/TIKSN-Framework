using System;
using System.Threading.Tasks;

namespace TIKSN.Analytics.Telemetry
{
    public interface IExceptionTelemeter
    {
        Task TrackException(Exception exception);

        Task TrackException(Exception exception, TelemetrySeverityLevel severityLevel);
    }
}