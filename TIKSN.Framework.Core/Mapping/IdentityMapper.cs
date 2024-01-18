namespace TIKSN.Mapping;

public sealed class IdentityMapper<T> : IMapper<T, T>, IAsyncMapper<T, T>
{
    public T Map(T source) => source;

    public Task<T> MapAsync(T source, CancellationToken cancellationToken)
        => Task.FromResult(source);
}
