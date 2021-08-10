using System.Threading;
using System.Threading.Tasks;

namespace TIKSN.Web.Rest
{
    public interface IRestRequester
    {
        Task<TResult> Request<TResult, TRequest>(TRequest request, CancellationToken cancellationToken);
    }
}
