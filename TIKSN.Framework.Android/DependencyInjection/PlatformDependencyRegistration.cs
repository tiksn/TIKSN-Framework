using Microsoft.Extensions.DependencyInjection;

namespace TIKSN.DependencyInjection
{
	public static class PlatformDependencyRegistration
	{
		public static void Register(IServiceCollection services)
		{
			DependencyRegistration.Register(services);
		}
	}
}