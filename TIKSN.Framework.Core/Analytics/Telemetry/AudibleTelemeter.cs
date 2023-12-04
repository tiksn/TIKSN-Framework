using TIKSN.Speech;

namespace TIKSN.Analytics.Telemetry;

public class AudibleTelemeter : IEventTelemeter, IExceptionTelemeter, IMetricTelemeter, ITraceTelemeter
{
    private readonly ITextToSpeechService textToSpeechService;

    public AudibleTelemeter(ITextToSpeechService textToSpeechService) => this.textToSpeechService =
        textToSpeechService ?? throw new ArgumentNullException(nameof(textToSpeechService));

    public Task TrackEventAsync(string name) => this.textToSpeechService.SpeakAsync($"Event {name} occurred.");

    public Task TrackEventAsync(string name, IReadOnlyDictionary<string, string> properties) =>
        this.textToSpeechService.SpeakAsync($"Event {name} occurred.");

    public Task TrackExceptionAsync(Exception exception) => this.textToSpeechService.SpeakAsync(exception.Message);

    public Task TrackExceptionAsync(Exception exception, TelemetrySeverityLevel severityLevel) =>
        this.textToSpeechService.SpeakAsync($"{severityLevel}. {exception.Message}");

    public Task TrackMetricAsync(string metricName, decimal metricValue) =>
        this.textToSpeechService.SpeakAsync($"Metric {metricName} is {metricValue}.");

    public Task TrackTraceAsync(string message) => this.textToSpeechService.SpeakAsync(message);

    public Task TrackTraceAsync(string message, TelemetrySeverityLevel severityLevel) =>
        this.textToSpeechService.SpeakAsync($"{severityLevel} {message}");
}
