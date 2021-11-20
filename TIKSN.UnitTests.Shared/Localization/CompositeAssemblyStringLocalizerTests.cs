using System;
using System.Linq;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Serilog;
using TIKSN.DependencyInjection;
using Xunit;
using Xunit.Abstractions;

namespace TIKSN.Localization.Tests
{
    public class CompositeAssemblyStringLocalizerTests
    {
        private readonly IServiceProvider _serviceProvider;

        public CompositeAssemblyStringLocalizerTests(ITestOutputHelper testOutputHelper)
        {
            var services = new ServiceCollection();
            _ = services.AddFrameworkCore();
            _ = services.AddLogging(builder =>
            {
                _ = builder.AddDebug();
                var loggger = new LoggerConfiguration()
                    .MinimumLevel.Verbose()
                    .WriteTo.TestOutput(testOutputHelper)
                    .CreateLogger();
                _ = builder.AddSerilog(loggger);
            });
            this._serviceProvider = services.BuildServiceProvider();
        }

        [Fact]
        public void KeyUniqueness()
        {
            var resourceNamesCache = new ResourceNamesCache();
            var testStringLocalizer = new TestStringLocalizer(resourceNamesCache, this._serviceProvider.GetRequiredService<ILogger<TestStringLocalizer>>());
            var duplicates = testStringLocalizer
                .GetAllStrings()
                .GroupBy(item => item.Name.ToLowerInvariant())
                .Where(item => item.Count() > 1)
                .ToArray();

            var duplicatesCount = duplicates.Length;

            _ = duplicatesCount.Should().Be(0);
        }
    }
}
