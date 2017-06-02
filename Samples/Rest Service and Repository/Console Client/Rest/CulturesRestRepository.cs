using Common_Models;
using System.Collections.Generic;
using TIKSN.Web.Rest;
using Microsoft.Extensions.Options;
using System.Threading;
using System.Threading.Tasks;
using System;

namespace Console_Client.Rest
{
    public class CulturesRestRepository : RestRepository<CultureModel, int>, ICulturesRestRepository
    {
        public CulturesRestRepository(IHttpClientFactory httpClientFactory, ISerializerRestFactory serializerRestFactory, IDeserializerRestFactory deserializerRestFactory, IRestAuthenticationTokenProvider restAuthenticationTokenProvider, IOptions<RestRepositoryOptions<CultureModel>> options) : base(httpClientFactory, serializerRestFactory, deserializerRestFactory, restAuthenticationTokenProvider, options)
        {
        }

        public Task<IEnumerable<CultureModel>> GetAllAsync(CancellationToken cancellationToken)
        {
            var parameters = new Dictionary<string, string>();

            return SearchAsync(parameters, cancellationToken);
        }

        public Task<CultureModel> GetByNameAsync(string name, CancellationToken cancellationToken = default(CancellationToken))
        {
            var parameters = new Dictionary<string, string>();

            parameters.Add("ID", name);

            return SingleOrDefaultAsync(parameters, cancellationToken);
        }
    }
}
