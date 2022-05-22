using System;
using System.Threading;
using System.Threading.Tasks;

namespace TIKSN.Data
{
    public interface IUnitOfWork : IDisposable, IAsyncDisposable
    {
        Task CompleteAsync(CancellationToken cancellationToken);

        Task DiscardAsync(CancellationToken cancellationToken);
    }
}
