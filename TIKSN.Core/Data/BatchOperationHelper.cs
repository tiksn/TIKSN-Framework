using LiteGuard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace TIKSN.Data
{
    public static class BatchOperationHelper
    {
        public static Task BatchOperationAsync<T>(IEnumerable<T> entities, CancellationToken cancellationToken, Func<T, CancellationToken, Task> singleOperation)
        {
            Guard.AgainstNullArgument(nameof(entities), entities);
            Guard.AgainstNullArgument(nameof(singleOperation), singleOperation);

            var tasks = entities.Select(entity => singleOperation?.Invoke(entity, cancellationToken));

            return Task.WhenAll(tasks);
        }
    }
}