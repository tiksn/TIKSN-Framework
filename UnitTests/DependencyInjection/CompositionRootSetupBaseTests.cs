using Xunit;

namespace TIKSN.DependencyInjection.Tests
{
	public class CompositionRootSetupBaseTests
    {
		[Fact]
		public void OptionsValidationForNonParameterLessPropertyType()
		{
			var compositionRoot = new TestCompositionRootSetup();
			compositionRoot.CreateServiceProvider();
		}
    }
}
