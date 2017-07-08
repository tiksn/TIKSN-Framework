using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Localization;
using TIKSN.Globalization;
using TIKSN.Serialization;
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

			services.TryAddSingleton<ICultureFactory, CultureFactory>();
			services.TryAddSingleton<ICurrencyFactory, CurrencyFactory>();
			services.TryAddSingleton<IRegionFactory, RegionFactory>();
			services.TryAddSingleton<IResourceNamesCache, ResourceNamesCache>();
			services.TryAddSingleton<ITimeProvider, TimeProvider>();

			services.TryAddSingleton<DotNetXmlDeserializer, DotNetXmlDeserializer>();
			services.TryAddSingleton<DotNetXmlSerializer, DotNetXmlSerializer>();
			services.TryAddSingleton<IRestRequester, RestRequester>();
			services.TryAddSingleton<JsonDeserializer, JsonDeserializer>();
			services.TryAddSingleton<JsonSerializer, JsonSerializer>();

			services.TryAddScoped<IShellCommandContext, ShellCommandContext>();
		}
	}
}