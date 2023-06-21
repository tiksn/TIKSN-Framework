namespace TIKSN.Mapping
{
    public interface IMapper<TSource, TDestination>
    {
        TDestination Map(TSource source);
    }
}
