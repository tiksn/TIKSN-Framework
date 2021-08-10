using System;

namespace TIKSN.Data.Cache.Distributed
{
    public class DistributedCacheDecoratorOptions
    {
        public DateTimeOffset? AbsoluteExpiration { get; set; }
        public TimeSpan? AbsoluteExpirationRelativeToNow { get; set; }
        public TimeSpan? SlidingExpiration { get; set; }
    }

    public class DistributedCacheDecoratorOptions<T> : DistributedCacheDecoratorOptions
    {
    }
}
