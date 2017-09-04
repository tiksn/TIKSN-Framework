using System;
using System.Globalization;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using TIKSN.Finance;
using TIKSN.Data.Cache.Memory;

namespace TIKSN.Globalization
{
	public class CurrencyFactory : MemoryCacheDecoratorBase<CurrencyInfo>, ICurrencyFactory
	{
		private readonly IRegionFactory _regionFactory;
		private readonly IOptions<RegionalCurrencyRedirectionOptions> _regionalCurrencyRedirectionOptions;

		public CurrencyFactory(IMemoryCache memoryCache, IRegionFactory regionFactory, IOptions<RegionalCurrencyRedirectionOptions> regionalCurrencyRedirectionOptions, IOptions<MemoryCacheDecoratorOptions> genericOptions, IOptions<MemoryCacheDecoratorOptions<CurrencyInfo>> specificOptions) : base(memoryCache, genericOptions, specificOptions)
		{
			_regionFactory = regionFactory;
			_regionalCurrencyRedirectionOptions = regionalCurrencyRedirectionOptions;
		}

		public CurrencyInfo Create(string isoCurrencySymbol)
		{
			var cacheKey = Tuple.Create(entityType, isoCurrencySymbol.ToUpperInvariant());

			return GetFromMemoryCache(cacheKey, () => new CurrencyInfo(isoCurrencySymbol));
		}

		public CurrencyInfo Create(RegionInfo region)
		{
			if (_regionalCurrencyRedirectionOptions.Value.RegionalCurrencyRedirections.ContainsKey(region.Name))
				region = _regionFactory.Create(_regionalCurrencyRedirectionOptions.Value.RegionalCurrencyRedirections[region.Name]);
			else if (_regionalCurrencyRedirectionOptions.Value.RegionalCurrencyRedirections.ContainsKey(region.TwoLetterISORegionName))
				region = _regionFactory.Create(_regionalCurrencyRedirectionOptions.Value.RegionalCurrencyRedirections[region.TwoLetterISORegionName]);

			var cacheKey = Tuple.Create(entityType, region.ISOCurrencySymbol);

			return GetFromMemoryCache(cacheKey, () => new CurrencyInfo(region));
		}
	}
}
