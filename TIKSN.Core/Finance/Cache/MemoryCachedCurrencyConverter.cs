using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using TIKSN.Data.Cache.Memory;

namespace TIKSN.Finance.Cache
{
    public class MemoryCachedCurrencyConverter : MemoryCacheDecoratorBase<MemoryCachedCurrencyConverterEntry>,
        ICurrencyConverter
    {
        private static readonly Type memoryCachedCurrencyConverterEntryType =
            typeof(MemoryCachedCurrencyConverterEntry);

        private readonly ILogger<MemoryCachedCurrencyConverter> _logger;
        private new readonly IMemoryCache _memoryCache;
        private readonly IOptions<MemoryCachedCurrencyConverterOptions> _options;
        private readonly ICurrencyConverter _originalConverter;
        private readonly Guid instanceID;

        public MemoryCachedCurrencyConverter(
            ICurrencyConverter originalConverter,
            ILogger<MemoryCachedCurrencyConverter> logger,
            IMemoryCache memoryCache,
            IOptions<MemoryCachedCurrencyConverterOptions> options,
            IOptions<MemoryCacheDecoratorOptions> genericOptions,
            IOptions<MemoryCacheDecoratorOptions<MemoryCachedCurrencyConverterEntry>> specificOptions)
            : base(memoryCache, genericOptions, specificOptions)
        {
            this._options = options;
            this._originalConverter = originalConverter;
            this._memoryCache = memoryCache;
            this.instanceID = Guid.NewGuid();
            this._logger = logger;
        }

        public async Task<Money> ConvertCurrencyAsync(Money baseMoney, CurrencyInfo counterCurrency,
            DateTimeOffset asOn, CancellationToken cancellationToken)
        {
            var cacheKey = Tuple.Create(memoryCachedCurrencyConverterEntryType,
                MemoryCachedCurrencyConverterEntryKind.ExchangeRate, this.instanceID, this.GetCacheIntervalKey(asOn),
                baseMoney.Currency, counterCurrency);

            var cacheEntry = await this.GetFromMemoryCacheAsync(cacheKey,
                () => this.OriginalConvertCurrencyAsync(baseMoney, counterCurrency, asOn, cancellationToken)).ConfigureAwait(false);

            return new Money(counterCurrency, baseMoney.Amount * cacheEntry.ExchangeRate);
        }

        public async Task<IEnumerable<CurrencyPair>> GetCurrencyPairsAsync(DateTimeOffset asOn,
            CancellationToken cancellationToken)
        {
            var cacheKey = Tuple.Create(memoryCachedCurrencyConverterEntryType,
                MemoryCachedCurrencyConverterEntryKind.CurrencyPairs, this.instanceID, this.GetCacheIntervalKey(asOn));

            var cacheEntry = await this.GetFromMemoryCacheAsync(cacheKey,
                () => this.GetOriginalCurrencyPairsAsync(asOn, cancellationToken)).ConfigureAwait(false);

            return cacheEntry.CurrencyPairs;
        }

        public async Task<decimal> GetExchangeRateAsync(CurrencyPair pair, DateTimeOffset asOn,
            CancellationToken cancellationToken)
        {
            var cacheKey = Tuple.Create(memoryCachedCurrencyConverterEntryType,
                MemoryCachedCurrencyConverterEntryKind.ExchangeRate, this.instanceID, this.GetCacheIntervalKey(asOn),
                pair.BaseCurrency, pair.CounterCurrency);

            var cacheEntry = await this.GetFromMemoryCacheAsync(cacheKey,
                () => this.GetOriginalExchangeRateAsync(pair, asOn, cancellationToken)).ConfigureAwait(false);

            return cacheEntry.ExchangeRate;
        }

        private long GetCacheIntervalKey(DateTimeOffset asOn)
        {
            if (this._options.Value.CacheInterval.Ticks == 0L)
            {
                this._logger.LogWarning("CacheInterval is 0, which makes caching redundant.");

                return 0L;
            }

            return asOn.Ticks / this._options.Value.CacheInterval.Ticks;
        }

        private async Task<MemoryCachedCurrencyConverterEntry> GetOriginalCurrencyPairsAsync(DateTimeOffset asOn,
            CancellationToken cancellationToken)
        {
            var currencyPairs = await this._originalConverter.GetCurrencyPairsAsync(asOn, cancellationToken).ConfigureAwait(false);

            return MemoryCachedCurrencyConverterEntry.CreateForCurrencyPairs(currencyPairs);
        }

        private async Task<MemoryCachedCurrencyConverterEntry> GetOriginalExchangeRateAsync(CurrencyPair pair,
            DateTimeOffset asOn, CancellationToken cancellationToken)
        {
            var exchangeRate = await this._originalConverter.GetExchangeRateAsync(pair, asOn, cancellationToken).ConfigureAwait(false);

            return MemoryCachedCurrencyConverterEntry.CreateForExchangeRate(exchangeRate);
        }

        private async Task<MemoryCachedCurrencyConverterEntry> OriginalConvertCurrencyAsync(Money baseMoney,
            CurrencyInfo counterCurrency, DateTimeOffset asOn, CancellationToken cancellationToken)
        {
            var convertedMoney =
                await this._originalConverter.ConvertCurrencyAsync(baseMoney, counterCurrency, asOn, cancellationToken).ConfigureAwait(false);

            var exchangeRate = convertedMoney.Amount / baseMoney.Amount;

            return MemoryCachedCurrencyConverterEntry.CreateForExchangeRate(exchangeRate);
        }
    }
}
