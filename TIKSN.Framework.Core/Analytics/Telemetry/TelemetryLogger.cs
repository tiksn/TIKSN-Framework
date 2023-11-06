using Microsoft.Extensions.Logging;

namespace TIKSN.Analytics.Telemetry
{
    public class TelemetryLogger : IEventTelemeter, IExceptionTelemeter, IMetricTelemeter, ITraceTelemeter
    {
        private readonly ILogger<TelemetryLogger> logger;

        public TelemetryLogger(ILogger<TelemetryLogger> logger) => this.logger = logger;

        public Task TrackEventAsync(string name)
        {
            this.logger.LogInformation(
                1064151770,
                "Telemetry Event '{EventName}' tracked.",
                name);

            return Task.CompletedTask;
        }

        public Task TrackEventAsync(string name, IReadOnlyDictionary<string, string> properties)
        {
            this.logger.LogInformation(
                1781631658,
                "Telemetry Event '{EventName}' tracked. with properties {EventProperties}",
                name,
                string.Join(' ', properties.Select(item => string.Format("{0} is {1}", item.Key, item.Value))));

            return Task.CompletedTask;
        }

        public Task TrackExceptionAsync(Exception exception)
        {
            this.logger.LogError(
                1206969841,
                exception,
                "Telemetry Exception of type '{ExceptionType}' is tracked.",
                exception.GetType().FullName);

            return Task.CompletedTask;
        }

        public Task TrackExceptionAsync(Exception exception, TelemetrySeverityLevel severityLevel)
        {
            this.logger.LogError(1215703672, exception,
                "Telemetry Exception of type '{0}' is tracked with severity level of '{1}'.",
                exception.GetType().FullName, severityLevel);

            return Task.CompletedTask;
        }

        public Task TrackMetricAsync(string metricName, decimal metricValue)
        {
            this.logger.LogInformation(
                635392194,
                "Telemetry Metric '{MetricName}' tracked with value {MetricValue}.",
                metricName, metricValue);

            return Task.CompletedTask;
        }

        public Task TrackTraceAsync(string message)
        {
            this.logger.LogTrace(
                1728345664,
                "Telemetry Trace is tracked with message: {TraceMessage}.",
                message);

            return Task.CompletedTask;
        }

        public Task TrackTraceAsync(string message, TelemetrySeverityLevel severityLevel)
        {
            this.logger.LogTrace(
                93981428,
                "Telemetry Trace is tracked with severity level of '{TraceLevel}' and message: {TraceMessage}.",
                severityLevel, message);

            return Task.CompletedTask;
        }
    }
}
