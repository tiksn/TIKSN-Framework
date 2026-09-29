using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using OpenTelemetry.Logs;
using TIKSN.Diagnostics.Logging;
using Xunit;

namespace TIKSN.Tests.Diagnostics.Logging;

public class EnrichLogRecordWithEventIdProcessorTests
{
    [Fact]
    public void Given_LogRecordWithEventId_When_OnEnd_Then_EventIdAndNameAppended()
    {
        // Arrange
        var exportedItems = new List<LogRecord>();
        using var loggerFactory = LoggerFactory.Create(builder => builder.AddOpenTelemetry(options =>
        {
            options.AddEventIdEnrichment();
            options.AddInMemoryExporter(exportedItems);
        }));

        var logger = loggerFactory.CreateLogger<EnrichLogRecordWithEventIdProcessorTests>();

        // Act
        var eventId = new EventId(42, "TestEventName");
#pragma warning disable CA1848 // For improved performance, use the LoggerMessage delegates
        logger.LogInformation(eventId, "Test message");
#pragma warning restore CA1848 // For improved performance, use the LoggerMessage delegates

        // Force flush
        loggerFactory.Dispose();

        // Assert
        var logRecord = Assert.Single(exportedItems);

        Assert.NotNull(logRecord.Attributes);
        var attributes = logRecord.Attributes.ToList();

        Assert.Contains(attributes, a => a.Key == "event.id" && a.Value is 42);
        Assert.Contains(attributes, a => a.Key == "event.name" && a.Value is "TestEventName");
    }

    [Fact]
    public void Given_LogRecordWithoutEventId_When_OnEnd_Then_NoEventIdAppended()
    {
        // Arrange
        var exportedItems = new List<LogRecord>();
        using var loggerFactory = LoggerFactory.Create(builder => builder.AddOpenTelemetry(options =>
        {
            options.AddEventIdEnrichment();
            options.AddInMemoryExporter(exportedItems);
        }));

        var logger = loggerFactory.CreateLogger<EnrichLogRecordWithEventIdProcessorTests>();

        // Act
#pragma warning disable CA1848 // For improved performance, use the LoggerMessage delegates
        logger.LogInformation("Test message without event id");
#pragma warning restore CA1848 // For improved performance, use the LoggerMessage delegates

        // Force flush
        loggerFactory.Dispose();

        // Assert
        var logRecord = Assert.Single(exportedItems);

        var attributes = logRecord.Attributes?.ToList() ?? [];

        Assert.DoesNotContain(attributes, a => a.Key == "event.id");
        Assert.DoesNotContain(attributes, a => a.Key == "event.name");
    }
}
