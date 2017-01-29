using Microsoft.Extensions.DependencyInjection;

namespace TIKSN.DependencyInjection
{
	public abstract class PlatformCompositionRootSetupBase : CompositionRootSetupBase
	{
		protected override void ConfigureServices(IServiceCollection services)
		{
			PlatformDependencyRegistration.Register(services);
		}
	}
}
