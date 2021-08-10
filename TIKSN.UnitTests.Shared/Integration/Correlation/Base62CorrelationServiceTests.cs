using System;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using TIKSN.DependencyInjection;
using Xunit;
using Xunit.Abstractions;

namespace TIKSN.Integration.Correlation.Tests
{
    public class Base62CorrelationServiceTests
    {
        private readonly ICorrelationService _correlationService;
        private readonly ITestOutputHelper _testOutputHelper;

        public Base62CorrelationServiceTests(ITestOutputHelper testOutputHelper)
        {
            var services = new ServiceCollection();
            _ = services.AddFrameworkPlatform();
            _ = services.AddSingleton<ICorrelationService, Base62CorrelationService>();
            var serviceProvider = services.BuildServiceProvider();
            this._correlationService = serviceProvider.GetRequiredService<ICorrelationService>();
            this._testOutputHelper = testOutputHelper ?? throw new ArgumentNullException(nameof(testOutputHelper));
        }

        [Fact]
        public void GenerateAndParse()
        {
            var correlationID = this._correlationService.Generate();
            this.LogOutput(correlationID, nameof(correlationID));
            var correlationIDFromString = this._correlationService.Create(correlationID.ToString());
            this.LogOutput(correlationIDFromString, nameof(correlationIDFromString));
            var correlationIDFromBytes = this._correlationService.Create(correlationID.ToByteArray());
            this.LogOutput(correlationIDFromBytes, nameof(correlationIDFromBytes));

            _ = correlationIDFromString.Should().Be(correlationID);
            _ = correlationIDFromBytes.Should().Be(correlationID);
            _ = correlationIDFromString.Should().Be(correlationIDFromBytes);
        }

        [Fact]
        public void SampleTest()
        {
            var correlationIDFromString = this._correlationService.Create("5Rq2J6PNGnciW2thvTHQTa");
            this.LogOutput(correlationIDFromString, nameof(correlationIDFromString));
            var bytes = correlationIDFromString.ToByteArray();
            var hex = BitConverter.ToString(bytes);

            _ = hex.Should().Be("B3-09-A6-C1-6E-56-F0-6C-03-B2-AE-47-9B-A5-E7-FA");
        }

        private void LogOutput(CorrelationID correlationID, string name)
        {
            this._testOutputHelper.WriteLine("-------------------------");
            this._testOutputHelper.WriteLine(name);
            this._testOutputHelper.WriteLine(correlationID.ToString());
            this._testOutputHelper.WriteLine(BitConverter.ToString(correlationID.ToByteArray()));
            this._testOutputHelper.WriteLine("");
        }
    }
}
