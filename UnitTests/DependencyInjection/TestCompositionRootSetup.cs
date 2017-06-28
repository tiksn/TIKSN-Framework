using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace TIKSN.DependencyInjection.Tests
{
	public class TestCompositionRootSetup : CompositionRootSetupBase
	{
		protected override void ConfigureOptions(IServiceCollection services, IConfigurationRoot configuration)
		{
			services.Configure<TestOptions>(configuration);
		}

		protected override void ConfigureServices(IServiceCollection services)
		{
		}
	}
}
