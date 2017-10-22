using Microsoft.Extensions.DependencyInjection;
using Xunit;
using Xunit.Abstractions;

namespace TIKSN.DependencyInjection.Tests
{
    public class CompositionRootSetupBaseTests
    {
        private readonly TestCompositionRootSetup _compositionRoot;

        public CompositionRootSetupBaseTests(ITestOutputHelper testOutputHelper)
        {
            _compositionRoot = new TestCompositionRootSetup(testOutputHelper, configureOptions: (s, c) => s.Configure<TestOptions>(c));
        }

        [Fact]
        public void OptionsValidationForNonParameterLessPropertyType()
        {
            _compositionRoot.CreateServiceProvider();
        }
    }
}