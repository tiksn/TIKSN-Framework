using System;
using System.Threading;
using System.Threading.Tasks;

namespace TIKSN.Data
{
    public interface IUnitOfWork : IDisposable
    {
        Task CompleteAsync(CancellationToken cancellationToken);
    }
}