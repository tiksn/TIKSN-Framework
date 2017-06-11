using Common_Models;
using System.Collections.Generic;
using TIKSN.Web.Rest;
using Microsoft.Extensions.Options;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Localization;
using TIKSN.Analytics.Telemetry;

namespace Console_Client.Rest
{
    public class CulturesRestRepository : RestRepository<CultureModel, int>, ICulturesRestRepository
    {
        public CulturesRestRepository(IHttpClientFactory httpClientFactory,
			ISerializerRestFactory serializerRestFactory,
			IDeserializerRestFactory deserializerRestFactory,
			IRestAuthenticationTokenProvider restAuthenticationTokenProvider,
			IOptions<RestRepositoryOptions<CultureModel>> options,
			IStringLocalizer stringLocalizer,
			ITraceTelemeter traceTelemeter) : base(httpClientFactory,
				serializerRestFactory,
				deserializerRestFactory,
				restAuthenticationTokenProvider,
				options,
				stringLocalizer,
				traceTelemeter)
        {
        }

        public Task<IEnumerable<CultureModel>> GetAllAsync(CancellationToken cancellationToken)
        {
            var parameters = new Dictionary<string, string>();

            parameters.Add("ID", string.Empty);

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
