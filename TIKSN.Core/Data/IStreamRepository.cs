namespace TIKSN.Data
{
    public interface IStreamRepository<out T>
    {
        IAsyncEnumerable<T> StreamAllAsync(CancellationToken cancellationToken);
    }
}
