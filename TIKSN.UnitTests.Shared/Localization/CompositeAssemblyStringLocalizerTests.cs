using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using TIKSN.DependencyInjection.Tests;
using Xunit;
using Xunit.Abstractions;

namespace TIKSN.Localization.Tests
{
    public class CompositeAssemblyStringLocalizerTests
    {
        private readonly IServiceProvider _serviceProvider;

        public CompositeAssemblyStringLocalizerTests(ITestOutputHelper testOutputHelper)
        {
            var compositionRoot = new TestCompositionRootSetup(testOutputHelper);
            _serviceProvider = compositionRoot.CreateServiceProvider();
        }

        [Fact]
        public void KeyUniqueness()
        {
            var resourceNamesCache = new ResourceNamesCache();
            var testStringLocalizer = new TestStringLocalizer(resourceNamesCache, _serviceProvider.GetRequiredService<ILogger<TestStringLocalizer>>());
            var duplicatesCount = testStringLocalizer.GetAllStrings().GroupBy(item => item.Name.ToLowerInvariant()).Count(item => item.Count() > 1);

            duplicatesCount.Should().Be(0);
        }
    }
}