using System.Threading;
using System.Threading.Tasks;

namespace TIKSN.Data.Mongo
{
    public interface IMongoUnitOfWorkFactory
    {
        Task<IMongoUnitOfWork> CreateAsync(CancellationToken cancellationToken);
    }
}
