using Common_Models;
using System;
using System.Collections.Generic;
using TIKSN.Web.Rest;
using Microsoft.Extensions.Options;
using System.Threading;
using System.Threading.Tasks;

namespace Console_Client.Rest
{
	public class CulturesRestRepository : RestRepository<CultureModel, int>, ICulturesRestRepository
	{
		public CulturesRestRepository(IHttpClientFactory httpClientFactory, ISerializerRestFactory serializerRestFactory, IDeserializerRestFactory deserializerRestFactory, IRestAuthenticationTokenProvider restAuthenticationTokenProvider, IOptions<RestRepositoryOptions<CultureModel>> options) : base(httpClientFactory, serializerRestFactory, deserializerRestFactory, restAuthenticationTokenProvider, options)
		{
		}

		public Task<IEnumerable<CultureModel>> GetAllAsync(CancellationToken cancellationToken = default(CancellationToken))
		{
			throw new NotImplementedException();
		}
	}
}
