using OpenTelemetry;
using OpenTelemetry.Logs;

namespace TIKSN.Diagnostics.Logging;

public class EnrichLogRecordWithEventIdProcessor : BaseProcessor<LogRecord>
{
    public override void OnEnd(LogRecord data)
    {
        ArgumentNullException.ThrowIfNull(data);

        if (data.EventId != 0)
        {
            var attributes = new List<KeyValuePair<string, object?>>(data.Attributes ?? [])
            {
                new("event.id", data.EventId.Id)
            };

            if (!string.IsNullOrEmpty(data.EventId.Name))
            {
                attributes.Add(new KeyValuePair<string, object?>("event.name", data.EventId.Name));
            }

            data.Attributes = attributes;
        }

        base.OnEnd(data);
    }
}
