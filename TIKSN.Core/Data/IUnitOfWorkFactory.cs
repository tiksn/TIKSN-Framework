using System.Threading;
using System.Threading.Tasks;

namespace TIKSN.Data
{
    public interface IUnitOfWorkFactory
    {
        Task<IUnitOfWork> CreateAsync(CancellationToken cancellationToken);
    }
}
