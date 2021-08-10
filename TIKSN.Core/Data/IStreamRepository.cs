using System.Collections.Generic;
using System.Threading;

namespace TIKSN.Data
{
    public interface IStreamRepository<T>
    {
        IAsyncEnumerable<T> StreamAllAsync(CancellationToken cancellationToken);
    }
}
