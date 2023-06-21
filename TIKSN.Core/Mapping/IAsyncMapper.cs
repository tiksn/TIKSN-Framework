namespace TIKSN.Mapping;

public interface IAsyncMapper<TSource, TDestination>
{
    Task<TDestination> MapAsync(TSource source, CancellationToken cancellationToken);
}
