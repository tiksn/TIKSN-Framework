using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using TIKSN.FileSystem;
using TIKSN.Network;
using TIKSN.Security.Antimalware;
using TIKSN.Settings;
using TIKSN.Shell;
using TIKSN.Speech;

namespace TIKSN.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddFrameworkPlatform(this IServiceCollection services)
        {
            services.AddFrameworkCore();

            services.TryAddSingleton<IAntimalwareScanner, AntimalwareScanner>();
            services.TryAddSingleton<IConsoleService, ConsoleService>();
            services.TryAddSingleton<IKnownFolders, KnownFolders>();
            services.TryAddSingleton<INetworkConnectivityService, NetworkConnectivityService>();
            services.TryAddSingleton<ISettingsService, WindowsRegistrySettingsService>();
            services.TryAddSingleton<IShellCommandEngine, ShellCommandEngine>();
            services.TryAddSingleton<ITextToSpeechService, TextToSpeechService>();

            return services;
        }
    }
}