using System;

namespace TIKSN.Data.Cache
{
	public class MemoryCacheDecoratorOptions
	{
		public DateTimeOffset? AbsoluteExpiration { get; set; }
		public TimeSpan? AbsoluteExpirationRelativeToNow { get; set; }
		public TimeSpan? SlidingExpiration { get; set; }
	}

	public class MemoryCacheDecoratorOptions<T> : MemoryCacheDecoratorOptions
	{
	}
}
