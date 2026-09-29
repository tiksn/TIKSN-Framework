using OpenTelemetry.Logs;

namespace TIKSN.Diagnostics.Logging;

public static class OpenTelemetryLoggingExtensions
{
    public static LoggerProviderBuilder AddEventIdEnrichment(this LoggerProviderBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);
        return builder.AddProcessor<EnrichLogRecordWithEventIdProcessor>();
    }

    public static OpenTelemetryLoggerOptions AddEventIdEnrichment(this OpenTelemetryLoggerOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);
#pragma warning disable CA2000 // Dispose objects before losing scope
        return options.AddProcessor(new EnrichLogRecordWithEventIdProcessor());
#pragma warning restore CA2000 // Dispose objects before losing scope
    }
}
