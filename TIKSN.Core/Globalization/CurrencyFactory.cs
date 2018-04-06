using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using System;
using System.Globalization;
using TIKSN.Data.Cache.Memory;
using TIKSN.Finance;

namespace TIKSN.Globalization
{
    public class CurrencyFactory : MemoryCacheDecoratorBase<CurrencyInfo>, ICurrencyFactory
    {
        private readonly IRegionFactory _regionFactory;
        private readonly IOptions<RegionalCurrencyRedirectionOptions> _regionalCurrencyRedirectionOptions;
        private readonly IOptions<CurrencyUnionRedirectionOptions> _currencyUnionRedirectionOptions;

        public CurrencyFactory(
            IMemoryCache memoryCache,
            IRegionFactory regionFactory,
            IOptions<RegionalCurrencyRedirectionOptions> regionalCurrencyRedirectionOptions,
            IOptions<CurrencyUnionRedirectionOptions> currencyUnionRedirectionOptions,
            IOptions<MemoryCacheDecoratorOptions> genericOptions,
            IOptions<MemoryCacheDecoratorOptions<CurrencyInfo>> specificOptions) : base(memoryCache, genericOptions, specificOptions)
        {
            _regionFactory = regionFactory;
            _regionalCurrencyRedirectionOptions = regionalCurrencyRedirectionOptions;
            _currencyUnionRedirectionOptions = currencyUnionRedirectionOptions;
        }

        public CurrencyInfo Create(string isoCurrencySymbol)
        {
            if (_currencyUnionRedirectionOptions.Value.CurrencyUnionRedirections.TryGetValue(isoCurrencySymbol, out string redirectedRegion))
                return Create(_regionFactory.Create(redirectedRegion));

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