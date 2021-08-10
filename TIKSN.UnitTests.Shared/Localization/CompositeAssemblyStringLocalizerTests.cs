using System;
using System.Linq;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
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
            this._serviceProvider = compositionRoot.CreateServiceProvider();
        }

        [Fact]
        public void KeyUniqueness()
        {
            var resourceNamesCache = new ResourceNamesCache();
            var testStringLocalizer = new TestStringLocalizer(resourceNamesCache, this._serviceProvider.GetRequiredService<ILogger<TestStringLocalizer>>());
            var duplicatesCount = testStringLocalizer.GetAllStrings().GroupBy(item => item.Name.ToLowerInvariant()).Count(item => item.Count() > 1);

            _ = duplicatesCount.Should().Be(0);
        }
    }
}
