using Microsoft.HockeyApp;

namespace TIKSN.Analytics.Telemetry
{
    public abstract class HockeyAppTelemeterBase
    {
        protected readonly IHockeyClient hockeyClient;

        public HockeyAppTelemeterBase(IHockeyClient hockeyClient)
        {
            this.hockeyClient = hockeyClient;
        }

        protected SeverityLevel ConvertToSeverityLevel(TelemetrySeverityLevel severityLevel)
        {
            switch (severityLevel)
            {
                case TelemetrySeverityLevel.Verbose:
                    return SeverityLevel.Verbose;

                case TelemetrySeverityLevel.Information:
                    return SeverityLevel.Information;

                case TelemetrySeverityLevel.Warning:
                    return SeverityLevel.Warning;

                case TelemetrySeverityLevel.Error:
                    return SeverityLevel.Error;

                case TelemetrySeverityLevel.Critical:
                    return SeverityLevel.Critical;

                default:
                    return SeverityLevel.Verbose;
            }
        }
    }
}