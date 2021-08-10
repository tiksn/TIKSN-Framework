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
            services.AddFrameworkPlatform();
            services.AddSingleton<ICorrelationService, Base62CorrelationService>();
            var serviceProvider = services.BuildServiceProvider();
            _correlationService = serviceProvider.GetRequiredService<ICorrelationService>();
            _testOutputHelper = testOutputHelper ?? throw new ArgumentNullException(nameof(testOutputHelper));
        }

        [Fact]
        public void GenerateAndParse()
        {
            var correlationID = _correlationService.Generate();
            LogOutput(correlationID, nameof(correlationID));
            var correlationIDFromString = _correlationService.Create(correlationID.ToString());
            LogOutput(correlationIDFromString, nameof(correlationIDFromString));
            var correlationIDFromBytes = _correlationService.Create(correlationID.ToByteArray());
            LogOutput(correlationIDFromBytes, nameof(correlationIDFromBytes));

            correlationIDFromString.Should().Be(correlationID);
            correlationIDFromBytes.Should().Be(correlationID);
            correlationIDFromString.Should().Be(correlationIDFromBytes);
        }

        [Fact]
        public void SampleTest()
        {
            var correlationIDFromString = _correlationService.Create("5Rq2J6PNGnciW2thvTHQTa");
            LogOutput(correlationIDFromString, nameof(correlationIDFromString));
            var bytes = correlationIDFromString.ToByteArray();
            var hex = BitConverter.ToString(bytes);

            hex.Should().Be("B3-09-A6-C1-6E-56-F0-6C-03-B2-AE-47-9B-A5-E7-FA");
        }

        private void LogOutput(CorrelationID correlationID, string name)
        {
            _testOutputHelper.WriteLine("-------------------------");
            _testOutputHelper.WriteLine(name);
            _testOutputHelper.WriteLine(correlationID.ToString());
            _testOutputHelper.WriteLine(BitConverter.ToString(correlationID.ToByteArray()));
            _testOutputHelper.WriteLine("");
        }
    }
}
