using System;
using System.Diagnostics.Contracts;
using System.Threading.Tasks;
using TIKSN.Speech;

namespace TIKSN.Analytics.Telemetry
{
    public class AudibleTelemeter : IEventTelemeter, IExceptionTelemeter, IMetricTelemeter, ITraceTelemeter
    {
        private readonly ITextToSpeechService _textToSpeechService;

        public AudibleTelemeter(ITextToSpeechService textToSpeechService)
        {
            Contract.Requires<ArgumentNullException>(textToSpeechService != null);

            _textToSpeechService = textToSpeechService;
        }

        public Task TrackEvent(string name)
        {
            return _textToSpeechService.SpeakAsync($"Event {name} occurred.");
        }

        public Task TrackException(Exception exception)
        {
            return _textToSpeechService.SpeakAsync(exception.Message);
        }

        public Task TrackException(Exception exception, TelemetrySeverityLevel severityLevel)
        {
            return _textToSpeechService.SpeakAsync($"{severityLevel}. {exception.Message}");
        }

        public Task TrackMetric(string metricName, decimal metricValue)
        {
            return _textToSpeechService.SpeakAsync($"Metric {metricName} is {metricValue}.");
        }

        public Task TrackTrace(string message)
        {
            return _textToSpeechService.SpeakAsync(message);
        }

        public Task TrackTrace(string message, TelemetrySeverityLevel severityLevel)
        {
            return _textToSpeechService.SpeakAsync($"{severityLevel} {message}");
        }
    }
}
