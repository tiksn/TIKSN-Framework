using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TIKSN.Speech;

namespace TIKSN.Analytics.Telemetry
{
    public class AudibleTelemeter : IEventTelemeter, IExceptionTelemeter, IMetricTelemeter, ITraceTelemeter
    {
        private readonly ITextToSpeechService _textToSpeechService;

        public AudibleTelemeter(ITextToSpeechService textToSpeechService) => this._textToSpeechService =
            textToSpeechService ?? throw new ArgumentNullException(nameof(textToSpeechService));

        public Task TrackEventAsync(string name) => this._textToSpeechService.SpeakAsync($"Event {name} occurred.");

        public Task TrackEventAsync(string name, IDictionary<string, string> properties) =>
            this._textToSpeechService.SpeakAsync($"Event {name} occurred.");

        public Task TrackExceptionAsync(Exception exception) => this._textToSpeechService.SpeakAsync(exception.Message);

        public Task TrackExceptionAsync(Exception exception, TelemetrySeverityLevel severityLevel) =>
            this._textToSpeechService.SpeakAsync($"{severityLevel}. {exception.Message}");

        public Task TrackMetricAsync(string metricName, decimal metricValue) =>
            this._textToSpeechService.SpeakAsync($"Metric {metricName} is {metricValue}.");

        public Task TrackTraceAsync(string message) => this._textToSpeechService.SpeakAsync(message);

        public Task TrackTraceAsync(string message, TelemetrySeverityLevel severityLevel) =>
            this._textToSpeechService.SpeakAsync($"{severityLevel} {message}");
    }
}
