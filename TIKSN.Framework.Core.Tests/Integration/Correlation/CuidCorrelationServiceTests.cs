using System;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using TIKSN.DependencyInjection;
using Xunit;
using Xunit.Abstractions;

namespace TIKSN.Integration.Correlation.Tests;

public class CuidCorrelationServiceTests
{
    private readonly ICorrelationService correlationService;
    private readonly ITestOutputHelper testOutputHelper;

    public CuidCorrelationServiceTests(ITestOutputHelper testOutputHelper)
    {
        var services = new ServiceCollection();
        _ = services.AddFrameworkPlatform();
        _ = services.AddSingleton<ICorrelationService, CuidCorrelationService>();
        var serviceProvider = services.BuildServiceProvider();
        this.correlationService = serviceProvider.GetRequiredService<ICorrelationService>();
        this.testOutputHelper = testOutputHelper ?? throw new ArgumentNullException(nameof(testOutputHelper));
    }

    [Fact]
    public void GenerateCoupleOfIds()
    {
        this.LogOutput(this.correlationService.Generate(), "Correlation ID 1");
        this.LogOutput(this.correlationService.Generate(), "Correlation ID 2");
        this.LogOutput(this.correlationService.Generate(), "Correlation ID 3");
    }
    [Fact]
    public void GenerateAndParse()
    {
        for (var i = 0; i < 10; i++)
        {
            _ = this.correlationService.Generate();
        }

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

    private void LogOutput(CorrelationId correlationID, string name)
    {
        this.testOutputHelper.WriteLine("-------------------------");
        this.testOutputHelper.WriteLine(name);
        this.testOutputHelper.WriteLine(correlationID.ToString());
        this.testOutputHelper.WriteLine(BitConverter.ToString(correlationID.ToBinary().ToArray()));
        this.testOutputHelper.WriteLine("");
    }

    [Fact]
    public void ParseExample()
    {
        var correlationID = this.correlationService.Create("ch72gsb320000udocl363eofy");

        this.LogOutput(correlationID, nameof(correlationID));
    }
}