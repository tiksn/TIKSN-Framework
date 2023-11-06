using System;
using System.Globalization;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using TIKSN.Data.Cache.Memory;
using TIKSN.Finance;

namespace TIKSN.Globalization
{
    public class CurrencyFactory : MemoryCacheDecoratorBase<CurrencyInfo>, ICurrencyFactory
    {
        private readonly IOptions<CurrencyUnionRedirectionOptions> _currencyUnionRedirectionOptions;
        private readonly IOptions<RegionalCurrencyRedirectionOptions> _regionalCurrencyRedirectionOptions;
        private readonly IRegionFactory _regionFactory;

        public CurrencyFactory(
            IMemoryCache memoryCache,
            IRegionFactory regionFactory,
            IOptions<RegionalCurrencyRedirectionOptions> regionalCurrencyRedirectionOptions,
            IOptions<CurrencyUnionRedirectionOptions> currencyUnionRedirectionOptions,
            IOptions<MemoryCacheDecoratorOptions> genericOptions,
            IOptions<MemoryCacheDecoratorOptions<CurrencyInfo>> specificOptions) : base(memoryCache, genericOptions,
            specificOptions)
        {
            this._regionFactory = regionFactory;
            this._regionalCurrencyRedirectionOptions = regionalCurrencyRedirectionOptions;
            this._currencyUnionRedirectionOptions = currencyUnionRedirectionOptions;
        }

        public CurrencyInfo Create(string isoCurrencySymbol)
        {
            if (this._currencyUnionRedirectionOptions.Value.CurrencyUnionRedirections.TryGetValue(isoCurrencySymbol,
                out var redirectedRegion))
            {
                return this.Create(this._regionFactory.Create(redirectedRegion));
            }

            var cacheKey = Tuple.Create(entityType, isoCurrencySymbol.ToUpperInvariant());

            return this.GetFromMemoryCache(cacheKey, () => new CurrencyInfo(isoCurrencySymbol));
        }

        public CurrencyInfo Create(RegionInfo region)
        {
            if (this._regionalCurrencyRedirectionOptions.Value.RegionalCurrencyRedirections.TryGetValue(region.Name, out var regionByName))
            {
                region = this._regionFactory.Create(regionByName);
            }
            else if (this._regionalCurrencyRedirectionOptions.Value.RegionalCurrencyRedirections.TryGetValue(region.TwoLetterISORegionName, out var regionByISOName))
            {
                region = this._regionFactory.Create(regionByISOName);
            }

            var cacheKey = Tuple.Create(entityType, region.ISOCurrencySymbol);

            return this.GetFromMemoryCache(cacheKey, () => new CurrencyInfo(region));
        }
    }
}
