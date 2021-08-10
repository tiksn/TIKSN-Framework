using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace TIKSN.Analytics.Telemetry
{
    public class TelemetryLogger : IEventTelemeter, IExceptionTelemeter, IMetricTelemeter, ITraceTelemeter
    {
        private readonly ILogger<TelemetryLogger> _logger;

        public TelemetryLogger(ILogger<TelemetryLogger> logger) => this._logger = logger;

        public Task TrackEventAsync(string name)
        {
            this._logger.LogInformation("Telemetry Event '{0}' tracked.", name);

            return Task.FromResult<object>(null);
        }

        public Task TrackEventAsync(string name, IDictionary<string, string> properties)
        {
            this._logger.LogInformation("Telemetry Event '{0}' tracked. with properties {1}", name,
                string.Join(" ", properties.Select(item => string.Format("{0} is {1}", item.Key, item.Value))));

            return Task.FromResult<object>(null);
        }

        public Task TrackExceptionAsync(Exception exception)
        {
            this._logger.LogError(1206969841, exception, "Telemetry Exception of type '{0}' is tracked.",
                exception.GetType().FullName);

            return Task.FromResult<object>(null);
        }

        public Task TrackExceptionAsync(Exception exception, TelemetrySeverityLevel severityLevel)
        {
            this._logger.LogError(1215703672, exception,
                "Telemetry Exception of type '{0}' is tracked with severity level of '{1}'.",
                exception.GetType().FullName, severityLevel);

            return Task.FromResult<object>(null);
        }

        public Task TrackMetricAsync(string metricName, decimal metricValue)
        {
            this._logger.LogInformation("Telemetry Metric '{0}' tracked with value {1}.", metricName, metricValue);

            return Task.FromResult<object>(null);
        }

        public Task TrackTraceAsync(string message)
        {
            this._logger.LogTrace("Telemetry Trace is tracked with message: {0}.", message);

            return Task.FromResult<object>(null);
        }

        public Task TrackTraceAsync(string message, TelemetrySeverityLevel severityLevel)
        {
            this._logger.LogTrace("Telemetry Trace is tracked with severity level of '{1}' and message: {0}.", message,
                severityLevel);

            return Task.FromResult<object>(null);
        }
    }
}
