namespace TIKSN.Data;

public static class BatchOperationHelper
{
    public static Task BatchOperationAsync<T>(
        IEnumerable<T> entities,
        Func<T, CancellationToken, Task> singleOperation,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(entities);

        ArgumentNullException.ThrowIfNull(singleOperation);

        var tasks = entities.Select(entity => singleOperation.Invoke(entity, cancellationToken));

        return Task.WhenAll(tasks);
    }

    public static Task<TResult[]> BatchOperationAsync<T, TResult>(
        IEnumerable<T> entities,
        Func<T, CancellationToken, Task<TResult>> singleOperation,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(entities);

        ArgumentNullException.ThrowIfNull(singleOperation);

        var tasks = entities.Select(entity => singleOperation.Invoke(entity, cancellationToken));

        return Task.WhenAll(tasks);
    }
}
