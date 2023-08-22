using System;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using TIKSN.DependencyInjection;
using Xunit;
using Xunit.Abstractions;

namespace TIKSN.Integration.Correlation.Tests;

public class Base62CorrelationServiceTests
{
    private readonly ICorrelationService correlationService;
    private readonly ITestOutputHelper testOutputHelper;

    public Base62CorrelationServiceTests(ITestOutputHelper testOutputHelper)
    {
        var services = new ServiceCollection();
        _ = services.AddFrameworkPlatform();
        _ = services.AddSingleton<ICorrelationService, Base62CorrelationService>();
        var serviceProvider = services.BuildServiceProvider();
        this.correlationService = serviceProvider.GetRequiredService<ICorrelationService>();
        this.testOutputHelper = testOutputHelper ?? throw new ArgumentNullException(nameof(testOutputHelper));
    }

    [Fact]
    public void GenerateAndParse()
    {
        var correlationID = this.correlationService.Generate();
        this.LogOutput(correlationID, nameof(correlationID));
        var correlationIDFromString = this.correlationService.Create(correlationID.ToString());
        this.LogOutput(correlationIDFromString, nameof(correlationIDFromString));
        var correlationIDFromBytes = this.correlationService.Create(correlationID.ToBinary());
        this.LogOutput(correlationIDFromBytes, nameof(correlationIDFromBytes));

        _ = correlationIDFromString.Should().Be(correlationID);
        _ = correlationIDFromBytes.Should().Be(correlationID);
        _ = correlationIDFromString.Should().Be(correlationIDFromBytes);
    }

    [Fact]
    public void SampleTest()
    {
        var correlationIDFromString = this.correlationService.Create("5Rq2J6PNGnciW2thvTHQTa");
        this.LogOutput(correlationIDFromString, nameof(correlationIDFromString));
        var bytes = correlationIDFromString.ToBinary();
        var hex = BitConverter.ToString(bytes.ToArray());

        _ = hex.Should().Be("B3-09-A6-C1-6E-56-F0-6C-03-B2-AE-47-9B-A5-E7-FA");
    }

    private void LogOutput(CorrelationId correlationID, string name)
    {
        this.testOutputHelper.WriteLine("-------------------------");
        this.testOutputHelper.WriteLine(name);
        this.testOutputHelper.WriteLine(correlationID.ToString());
        this.testOutputHelper.WriteLine(BitConverter.ToString(correlationID.ToBinary().ToArray()));
        this.testOutputHelper.WriteLine("");
    }
}
