using Microsoft.Extensions.DependencyInjection;
using TIKSN.DependencyInjection;

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
    }
}