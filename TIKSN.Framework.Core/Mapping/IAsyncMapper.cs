namespace TIKSN.Mapping;

public interface IAsyncMapper<TSource, TDestination>
{
    public Task<TDestination> MapAsync(TSource source, CancellationToken cancellationToken);
}
