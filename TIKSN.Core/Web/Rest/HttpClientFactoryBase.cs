using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace TIKSN.Web.Rest
{
	public abstract class HttpClientFactoryBase : IHttpClientFactory
	{
		public virtual Task<HttpClient> Create(Guid apiKey)
		{
			throw new KeyNotFoundException($"Http Client for API with key {apiKey} was not found.");
		}
	}
}