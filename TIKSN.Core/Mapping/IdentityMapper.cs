namespace TIKSN.Mapping
{
    public sealed class IdentityMapper<T> : IMapper<T, T>, IAsyncMapper<T, T>
    {
        private static readonly Lazy<IdentityMapper<T>> LazyInstance = new(() => new IdentityMapper<T>());

        private IdentityMapper()
        { }

        public static IdentityMapper<T> Instance => LazyInstance.Value;

        public T Map(T source) => source;

        public Task<T> MapAsync(T source, CancellationToken cancellationToken)
            => Task.FromResult(source);
    }
}
