using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using System;
using TIKSN.DependencyInjection;
using Xunit;
using Xunit.Abstractions;

namespace TIKSN.Integration.Correlation.Tests
{
    public class CuidCorrelationServiceTests
    {
        private readonly ICorrelationService _correlationService;
        private readonly ITestOutputHelper _testOutputHelper;

        public CuidCorrelationServiceTests(ITestOutputHelper testOutputHelper)
        {
            var services = new ServiceCollection();
            services.AddFrameworkPlatform();
            services.AddSingleton<ICorrelationService, CuidCorrelationService>();
            var serviceProvider = services.BuildServiceProvider();
            _correlationService = serviceProvider.GetRequiredService<ICorrelationService>();
            _testOutputHelper = testOutputHelper ?? throw new ArgumentNullException(nameof(testOutputHelper));
        }

        [Fact]
        public void GenerateCoupleOfIds()
        {
            LogOutput(_correlationService.Generate(), "Correlation ID 1");
            LogOutput(_correlationService.Generate(), "Correlation ID 2");
            LogOutput(_correlationService.Generate(), "Correlation ID 3");
        }
        [Fact]
        public void GenerateAndParse()
        {
            for (int i = 0; i < 10; i++)
            {
                _correlationService.Generate();
            }

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

        private void LogOutput(CorrelationID correlationID, string name)
        {
            _testOutputHelper.WriteLine("-------------------------");
            _testOutputHelper.WriteLine(name);
            _testOutputHelper.WriteLine(correlationID.ToString());
            _testOutputHelper.WriteLine(BitConverter.ToString(correlationID.ToByteArray()));
            _testOutputHelper.WriteLine("");
        }

        [Fact]
        public void ParseExample()
        {
            var correlationID = _correlationService.Create("ch72gsb320000udocl363eofy");

            LogOutput(correlationID, nameof(correlationID));
        }
    }
}