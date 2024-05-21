namespace TIKSN.Data;

public interface IUnitOfWorkFactory
{
    Task<IUnitOfWork> CreateAsync(CancellationToken cancellationToken);
}

public interface IUnitOfWorkFactory<TKey>
{
    Task<IUnitOfWork> CreateAsync(TKey key, CancellationToken cancellationToken);
}
