using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TIKSN.Analytics.Telemetry
{
    public class TelemetryLogger : IEventTelemeter, IExceptionTelemeter, IMetricTelemeter, ITraceTelemeter
    {
        private readonly ILogger<TelemetryLogger> _logger;

        public TelemetryLogger(ILogger<TelemetryLogger> logger)
        {
            _logger = logger;
        }

        public Task TrackEvent(string name)
        {
            _logger.LogInformation("Telemetry Event '{0}' tracked.", name);

            return Task.FromResult<object>(null);
        }

        public Task TrackEvent(string name, IDictionary<string, string> properties)
        {
            _logger.LogInformation("Telemetry Event '{0}' tracked. with properties {1}", name, string.Join(" ", properties.Select(item => string.Format("{0} is {1}", item.Key, item.Value))));

            return Task.FromResult<object>(null);
        }

        public Task TrackException(Exception exception)
        {
            _logger.LogError(1206969841, exception, "Telemetry Exception of type '{0}' is tracked.", exception.GetType().FullName);

            return Task.FromResult<object>(null);
        }

        public Task TrackException(Exception exception, TelemetrySeverityLevel severityLevel)
        {
            _logger.LogError(1215703672, exception, "Telemetry Exception of type '{0}' is tracked with severity level of '{1}'.", exception.GetType().FullName, severityLevel);

            return Task.FromResult<object>(null);
        }

        public Task TrackMetric(string metricName, decimal metricValue)
        {
            _logger.LogInformation("Telemetry Metric '{0}' tracked with value {1}.", metricName, metricValue);

            return Task.FromResult<object>(null);
        }

        public Task TrackTrace(string message)
        {
            _logger.LogTrace("Telemetry Trace is tracked with message: {0}.", message);

            return Task.FromResult<object>(null);
        }

        public Task TrackTrace(string message, TelemetrySeverityLevel severityLevel)
        {
            _logger.LogTrace("Telemetry Trace is tracked with severity level of '{1}' and message: {0}.", message, severityLevel);

            return Task.FromResult<object>(null);
        }
    }
}