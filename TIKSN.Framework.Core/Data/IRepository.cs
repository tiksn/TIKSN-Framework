namespace TIKSN.Data;

public interface IRepository<T>
{
    Task AddAsync(T entity, CancellationToken cancellationToken);

    Task AddRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken);

    Task RemoveAsync(T entity, CancellationToken cancellationToken);

    Task RemoveRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken);

    Task UpdateAsync(T entity, CancellationToken cancellationToken);

    Task UpdateRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken);
}
