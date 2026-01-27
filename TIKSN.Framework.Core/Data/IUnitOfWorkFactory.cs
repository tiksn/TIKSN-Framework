namespace TIKSN.Data;

public interface IUnitOfWorkFactory
{
    public Task<IUnitOfWork> CreateAsync(CancellationToken cancellationToken);
}

public interface IUnitOfWorkFactory<TKey>
{
    public Task<IUnitOfWork> CreateAsync(TKey key, CancellationToken cancellationToken);
}
