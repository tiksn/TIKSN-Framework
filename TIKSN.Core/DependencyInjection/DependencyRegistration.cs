using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Localization;
using System;
using TIKSN.Globalization;
using TIKSN.Serialization;
using TIKSN.Serialization.Bond;
using TIKSN.Serialization.MessagePack;
using TIKSN.Shell;
using TIKSN.Time;
using TIKSN.Web.Rest;

namespace TIKSN.DependencyInjection
{
	public static class DependencyRegistration
	{
		public static void Register(IServiceCollection services)
		{
			services.AddLocalization();
			services.AddLogging();
			services.AddMemoryCache();
			services.AddOptions();

			services.TryAddSingleton<IConsoleService, ConsoleService>();
			services.TryAddSingleton<ICultureFactory, CultureFactory>();
			services.TryAddSingleton<ICurrencyFactory, CurrencyFactory>();
			services.TryAddSingleton<IRegionFactory, RegionFactory>();
			services.TryAddSingleton<IResourceNamesCache, ResourceNamesCache>();
			services.TryAddSingleton<IShellCommandEngine, ShellCommandEngine>();
			services.TryAddSingleton<ITimeProvider, TimeProvider>();
			services.TryAddSingleton<Random>();
			services.TryAddSingleton(MsgPack.Serialization.SerializationContext.Default);

			services.TryAddSingleton<CompactBinaryBondDeserializer>();
			services.TryAddSingleton<CompactBinaryBondSerializer>();
			services.TryAddSingleton<DotNetXmlDeserializer>();
			services.TryAddSingleton<DotNetXmlSerializer>();
			services.TryAddSingleton<FastBinaryBondDeserializer>();
			services.TryAddSingleton<FastBinaryBondSerializer>();
			services.TryAddSingleton<IRestRequester, RestRequester>();
			services.TryAddSingleton<JsonDeserializer>();
			services.TryAddSingleton<JsonSerializer>();
			services.TryAddSingleton<MessagePackDeserializer>();
			services.TryAddSingleton<MessagePackSerializer>();
			services.TryAddSingleton<SimpleBinaryBondDeserializer>();
			services.TryAddSingleton<SimpleBinaryBondSerializer>();
			services.TryAddSingleton<SimpleJsonBondDeserializer>();
			services.TryAddSingleton<SimpleJsonBondSerializer>();
			services.TryAddSingleton<SimpleXmlBondDeserializer>();
			services.TryAddSingleton<SimpleXmlBondSerializer>();


			services.TryAddScoped<IShellCommandContext, ShellCommandContext>();
		}
	}
}