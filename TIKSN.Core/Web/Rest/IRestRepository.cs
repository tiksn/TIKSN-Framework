using System.Threading;
using System.Threading.Tasks;

namespace TIKSN.Web.Rest
{
	public interface IRestRepository<T>
	{
		Task<T> GetAsync(string id, CancellationToken cancellationToken = default(CancellationToken));
	}
}
