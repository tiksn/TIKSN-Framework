using System;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using TIKSN.DependencyInjection;
using TIKSN.Integration.Correlation;
using Xunit;
using Xunit.Abstractions;

namespace TIKSN.Tests.Integration.Correlation;

public class UlidCorrelationServiceTests
{
    private readonly ICorrelationService correlationService;
    private readonly ITestOutputHelper testOutputHelper;

    public UlidCorrelationServiceTests(ITestOutputHelper testOutputHelper)
    {
        var services = new ServiceCollection();
        _ = services.AddFrameworkCore();
        _ = services.AddSingleton<ICorrelationService, UlidCorrelationService>();
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

        correlationIDFromString.ShouldBe(correlationID);
        correlationIDFromBytes.ShouldBe(correlationID);
        correlationIDFromString.ShouldBe(correlationIDFromBytes);
    }

    private void LogOutput(CorrelationId correlationID, string name)
    {
        this.testOutputHelper.WriteLine("-------------------------");
        this.testOutputHelper.WriteLine(name);
        this.testOutputHelper.WriteLine(correlationID.ToString());
        this.testOutputHelper.WriteLine(BitConverter.ToString([.. correlationID.ToBinary()]));
        this.testOutputHelper.WriteLine("");
    }
}
