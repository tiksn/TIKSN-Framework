using System;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
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
            _ = services.AddFrameworkPlatform();
            _ = services.AddSingleton<ICorrelationService, CuidCorrelationService>();
            var serviceProvider = services.BuildServiceProvider();
            this._correlationService = serviceProvider.GetRequiredService<ICorrelationService>();
            this._testOutputHelper = testOutputHelper ?? throw new ArgumentNullException(nameof(testOutputHelper));
        }

        [Fact]
        public void GenerateCoupleOfIds()
        {
            this.LogOutput(this._correlationService.Generate(), "Correlation ID 1");
            this.LogOutput(this._correlationService.Generate(), "Correlation ID 2");
            this.LogOutput(this._correlationService.Generate(), "Correlation ID 3");
        }
        [Fact]
        public void GenerateAndParse()
        {
            for (var i = 0; i < 10; i++)
            {
                _ = this._correlationService.Generate();
            }

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

        private void LogOutput(CorrelationID correlationID, string name)
        {
            this._testOutputHelper.WriteLine("-------------------------");
            this._testOutputHelper.WriteLine(name);
            this._testOutputHelper.WriteLine(correlationID.ToString());
            this._testOutputHelper.WriteLine(BitConverter.ToString(correlationID.ToByteArray()));
            this._testOutputHelper.WriteLine("");
        }

        [Fact]
        public void ParseExample()
        {
            var correlationID = this._correlationService.Create("ch72gsb320000udocl363eofy");

            this.LogOutput(correlationID, nameof(correlationID));
        }
    }
}
