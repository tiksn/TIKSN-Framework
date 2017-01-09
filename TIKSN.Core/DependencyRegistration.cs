using Microsoft.Extensions.DependencyInjection;
using TIKSN.Serialization;
using TIKSN.Web.Rest;

namespace TIKSN
{
	public static class DependencyRegistration
	{
		public static void Register(IServiceCollection services)
		{
			services.AddSingleton<IRestRequester, RestRequester>();
			services.AddSingleton<JsonSerializer, JsonSerializer>();
			services.AddSingleton<JsonDeserializer, JsonDeserializer>();
			services.AddSingleton<DotNetXmlSerializer, DotNetXmlSerializer>();
			services.AddSingleton<DotNetXmlDeserializer, DotNetXmlDeserializer>();
		}
	}
}
