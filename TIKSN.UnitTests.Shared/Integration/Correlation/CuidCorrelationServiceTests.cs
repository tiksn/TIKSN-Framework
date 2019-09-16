using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using TIKSN.DependencyInjection;
using Xunit;

namespace TIKSN.Integration.Correlation.Tests
{
    public class CuidCorrelationServiceTests
    {
        private readonly ICorrelationService _correlationService;

        public CuidCorrelationServiceTests()
        {
            var services = new ServiceCollection();
            services.AddFrameworkPlatform();
            services.AddSingleton<ICorrelationService, CuidCorrelationService>();
            var serviceProvider = services.BuildServiceProvider();
            _correlationService = serviceProvider.GetRequiredService<ICorrelationService>();
        }

        [Fact]
        public void GenerateAndParse()
        {
            var correlationID = _correlationService.Generate();
            var correlationIDFromString = _correlationService.Create(correlationID.ToString());
            var correlationIDFromBytes = _correlationService.Create(correlationID.ToByteArray());

            correlationIDFromString.Should().Be(correlationID);
            correlationIDFromBytes.Should().Be(correlationID);
            correlationIDFromString.Should().Be(correlationIDFromBytes);
        }

        [Fact]
        public void ParseExample()
        {
            var cuid = _correlationService.Create("ch72gsb320000udocl363eofy");
        }
    }
}