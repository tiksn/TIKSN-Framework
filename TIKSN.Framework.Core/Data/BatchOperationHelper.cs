namespace TIKSN.Data;

public static class BatchOperationHelper
{
    public static Task BatchOperationAsync<T>(
        IEnumerable<T> entities,
        CancellationToken cancellationToken,
        Func<T, CancellationToken, Task> singleOperation)
    {
        ArgumentNullException.ThrowIfNull(entities);

        ArgumentNullException.ThrowIfNull(singleOperation);

        var tasks = entities.Select(entity => singleOperation.Invoke(entity, cancellationToken));

        return Task.WhenAll(tasks);
    }

    public static Task<TResult[]> BatchOperationAsync<T, TResult>(
        IEnumerable<T> entities,
        CancellationToken cancellationToken,
        Func<T, CancellationToken, Task<TResult>> singleOperation)
    {
        ArgumentNullException.ThrowIfNull(entities);

        ArgumentNullException.ThrowIfNull(singleOperation);

        var tasks = entities.Select(entity => singleOperation.Invoke(entity, cancellationToken));

        return Task.WhenAll(tasks);
    }
}
