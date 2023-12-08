namespace TIKSN.Data.Cache;

public abstract class CacheDecoratorBase<T>
{
    protected static readonly Type EntityType = typeof(T);
}
