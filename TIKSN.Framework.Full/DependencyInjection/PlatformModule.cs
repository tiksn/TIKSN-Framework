using Autofac;
using TIKSN.Network;
using TIKSN.Settings;
using TIKSN.Shell;
using TIKSN.Speech;

namespace TIKSN.DependencyInjection
{
	public class PlatformModule : Module
	{
		protected override void Load(ContainerBuilder builder)
		{
			builder.RegisterType<ConsoleService>().As<IConsoleService>().SingleInstance();
			builder.RegisterType<NetworkConnectivityService>().As<INetworkConnectivityService>().SingleInstance();
			builder.RegisterType<TextToSpeechService>().As<ITextToSpeechService>().SingleInstance();
			builder.RegisterType<WindowsRegistrySettingsService>().As<ISettingsService>().SingleInstance();
		}
	}
}
