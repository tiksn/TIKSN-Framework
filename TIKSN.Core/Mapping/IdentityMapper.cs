namespace TIKSN.Mapping
{
    public class IdentityMapper<T> : IMapper<T, T>
    {
        private static readonly Lazy<IdentityMapper<T>> LazyInstance = new(() => new IdentityMapper<T>());

        private IdentityMapper()
        { }

        public static IdentityMapper<T> Instance => LazyInstance.Value;

        public T Map(T source) => source;
    }
}
