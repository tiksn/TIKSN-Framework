using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Localization;
using TIKSN.Serialization;
using TIKSN.Shell;
using TIKSN.Web.Rest;

namespace TIKSN.DependencyInjection
{
	public static class DependencyRegistration
	{
		public static void Register(IServiceCollection services)
		{
			services.AddLocalization();
			services.AddLogging();
			services.AddOptions();

			services.TryAddSingleton<IResourceNamesCache, ResourceNamesCache>();

			services.TryAddSingleton<IRestRequester, RestRequester>();
			services.TryAddSingleton<DotNetXmlDeserializer, DotNetXmlDeserializer>();
			services.TryAddSingleton<DotNetXmlSerializer, DotNetXmlSerializer>();
			services.TryAddSingleton<JsonDeserializer, JsonDeserializer>();
			services.TryAddSingleton<JsonSerializer, JsonSerializer>();

			services.TryAddScoped<IShellCommandContext, ShellCommandContext>();
		}
	}
}