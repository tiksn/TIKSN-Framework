using System;
using System.Globalization;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using TIKSN.Data.Cache;
using TIKSN.Finance;

namespace TIKSN.Globalization
{
	public class CurrencyFactory : MemoryCacheDecoratorBase<CurrencyInfo>, ICurrencyFactory
	{
		public CurrencyFactory(IMemoryCache memoryCache, IOptions<MemoryCacheDecoratorOptions> genericOptions, IOptions<MemoryCacheDecoratorOptions<CurrencyInfo>> specificOptions) : base(memoryCache, genericOptions, specificOptions)
		{
		}

		public CurrencyInfo Create(string isoCurrencySymbol)
		{
			var cacheKey = Tuple.Create(entityType, isoCurrencySymbol.ToUpperInvariant());

			return GetFromMemoryCache(cacheKey, () => new CurrencyInfo(isoCurrencySymbol));
		}

		public CurrencyInfo Create(RegionInfo region)
		{
			var cacheKey = Tuple.Create(entityType, region.ISOCurrencySymbol);

			return GetFromMemoryCache(cacheKey, () => new CurrencyInfo(region));
		}
	}
}
