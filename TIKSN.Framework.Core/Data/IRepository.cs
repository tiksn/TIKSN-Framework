namespace TIKSN.Data;

public interface IRepository<in T>
{
    public Task AddAsync(T entity, CancellationToken cancellationToken);

    public Task AddRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken);

    public Task RemoveAsync(T entity, CancellationToken cancellationToken);

    public Task RemoveRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken);

    public Task UpdateAsync(T entity, CancellationToken cancellationToken);

    public Task UpdateRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken);
}
