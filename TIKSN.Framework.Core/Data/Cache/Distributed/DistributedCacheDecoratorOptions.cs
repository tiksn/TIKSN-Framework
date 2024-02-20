namespace TIKSN.Data.Cache.Distributed;

public class DistributedCacheDecoratorOptions
{
    public DateTimeOffset? AbsoluteExpiration { get; set; }
    public TimeSpan? AbsoluteExpirationRelativeToNow { get; set; }
    public TimeSpan? SlidingExpiration { get; set; }
}

#pragma warning disable S2094 // Classes should not be empty
#pragma warning disable S2326 // Unused type parameters should be removed
public class DistributedCacheDecoratorOptions<T> : DistributedCacheDecoratorOptions;
#pragma warning restore S2326 // Unused type parameters should be removed
#pragma warning restore S2094 // Classes should not be empty
