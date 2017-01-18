using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using TIKSN.Serialization;
using TIKSN.Web.Rest;

namespace TIKSN
{
	public static class DependencyRegistration
	{
		public static void Register(IServiceCollection services)
		{
            services.AddLocalization();
            services.AddLogging();
            services.AddOptions();

            services.TryAddSingleton<IRestRequester, RestRequester>();
			services.TryAddSingleton<DotNetXmlDeserializer, DotNetXmlDeserializer>();
			services.TryAddSingleton<DotNetXmlSerializer, DotNetXmlSerializer>();
			services.TryAddSingleton<JsonDeserializer, JsonDeserializer>();
			services.TryAddSingleton<JsonSerializer, JsonSerializer>();
		}
	}
}
