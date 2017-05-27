using Common_Models;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TIKSN.Web.Rest;

namespace Console_Client.Rest
{
	public interface ICulturesRestRepository : IRestRepository<CultureModel, int>
	{
		Task<IEnumerable<CultureModel>> GetAllAsync(CancellationToken cancellationToken = default(CancellationToken));
	}
}
