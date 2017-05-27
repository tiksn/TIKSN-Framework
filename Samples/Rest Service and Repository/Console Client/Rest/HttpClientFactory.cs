using Common_Models;
using Microsoft.Extensions.Options;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using TIKSN.Web;
using TIKSN.Web.Rest;

namespace Console_Client.Rest
{
	public class HttpClientFactory : HttpClientFactoryBase
	{
		private readonly IOptions<WebApiOptions<CultureModel>> cultureOptions;

		public HttpClientFactory(IOptions<WebApiOptions<CultureModel>> cultureOptions)
		{
			this.cultureOptions = cultureOptions;
		}

		public override Task<HttpClient> Create(Guid apiKey)
		{
			var client = new HttpClient();

			if (apiKey == cultureOptions.Value.ApiKey)
			{
				client.BaseAddress = cultureOptions.Value.BaseAddress;
				return Task.FromResult(client);
			}

			return base.Create(apiKey);
		}
	}
}
