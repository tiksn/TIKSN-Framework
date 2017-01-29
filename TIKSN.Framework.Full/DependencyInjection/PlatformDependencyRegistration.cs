using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using TIKSN.Network;
using TIKSN.Settings;
using TIKSN.Shell;
using TIKSN.Speech;

namespace TIKSN.DependencyInjection
{
	public static class PlatformDependencyRegistration
	{
		public static void Register(IServiceCollection services)
		{
			DependencyRegistration.Register(services);

			services.TryAddSingleton<IConsoleService, ConsoleService>();
			services.TryAddSingleton<INetworkConnectivityService, NetworkConnectivityService>();
			services.TryAddSingleton<ISettingsService, WindowsRegistrySettingsService>();
			services.TryAddSingleton<IShellCommandEngine, ShellCommandEngine>();
			services.TryAddSingleton<ITextToSpeechService, TextToSpeechService>();
		}
	}
}
