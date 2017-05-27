using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using TIKSN.Web.Rest;

namespace Console_Client.Rest
{
    public class HttpClientFactory : HttpClientFactoryBase
	{
		public override Task<HttpClient> Create(Guid apiKey)
		{
			return base.Create(apiKey);
		}
	}
}
