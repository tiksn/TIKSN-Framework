namespace TIKSN.Data
{
    public interface IMapper<TSource, TDestination>
    {
        TDestination Map(TSource source);
    }
}
