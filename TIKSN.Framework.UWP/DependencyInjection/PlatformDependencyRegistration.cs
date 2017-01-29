using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using TIKSN.Network;
using TIKSN.Settings;

namespace TIKSN.DependencyInjection
{
	public static class PlatformDependencyRegistration
	{
		public static void Register(IServiceCollection services)
		{
			DependencyRegistration.Register(services);

			services.TryAddSingleton<INetworkConnectivityService, NetworkConnectivityService>();
			services.TryAddSingleton<ISettingsService, SettingsService>();
		}
	}
}