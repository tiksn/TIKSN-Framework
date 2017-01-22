using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using TIKSN.Network;
using TIKSN.Settings;

namespace TIKSN
{
	public static class DependencyRegistrationUWP
	{
		public static void Register(IServiceCollection services)
		{
			services.TryAddSingleton<ISettingsService, SettingsService>();
			services.TryAddSingleton<INetworkConnectivityService, NetworkConnectivityService>();
		}
	}
}