namespace TIKSN.Data;

public interface IStreamRepository<out T>
{
    public IAsyncEnumerable<T> StreamAllAsync(CancellationToken cancellationToken);
}
