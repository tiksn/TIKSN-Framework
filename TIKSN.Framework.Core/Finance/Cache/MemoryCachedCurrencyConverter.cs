using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using TIKSN.Data.Cache.Memory;

namespace TIKSN.Finance.Cache;

public partial class MemoryCachedCurrencyConverter : MemoryCacheDecoratorBase<MemoryCachedCurrencyConverterEntry>,
    ICurrencyConverter
{
    private static readonly Type MemoryCachedCurrencyConverterEntryType =
        typeof(MemoryCachedCurrencyConverterEntry);

    private readonly Guid instanceID;
    private readonly ILogger<MemoryCachedCurrencyConverter> logger;
    private readonly IOptions<MemoryCachedCurrencyConverterOptions> options;
    private readonly ICurrencyConverter originalConverter;

    public MemoryCachedCurrencyConverter(
        ICurrencyConverter originalConverter,
        ILogger<MemoryCachedCurrencyConverter> logger,
        IMemoryCache memoryCache,
        IOptions<MemoryCachedCurrencyConverterOptions> options,
        IOptions<MemoryCacheDecoratorOptions> genericOptions,
        IOptions<MemoryCacheDecoratorOptions<MemoryCachedCurrencyConverterEntry>> specificOptions)
        : base(memoryCache, genericOptions, specificOptions)
    {
        this.options = options ?? throw new ArgumentNullException(nameof(options));
        this.originalConverter = originalConverter ?? throw new ArgumentNullException(nameof(originalConverter));
        this.instanceID = Guid.NewGuid();
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<Money> ConvertCurrencyAsync(
        Money baseMoney,
        CurrencyInfo counterCurrency,
        DateTimeOffset asOn,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(baseMoney);
        ArgumentNullException.ThrowIfNull(counterCurrency);

        var cacheKey = Tuple.Create(MemoryCachedCurrencyConverterEntryType,
            MemoryCachedCurrencyConverterEntryKind.ExchangeRate, this.instanceID, this.GetCacheIntervalKey(asOn),
            baseMoney.Currency, counterCurrency);

        var cacheEntry = await this.GetFromMemoryCacheAsync(cacheKey,
            () => this.OriginalConvertCurrencyAsync(baseMoney, counterCurrency, asOn, cancellationToken)).ConfigureAwait(false);

        return new Money(counterCurrency, baseMoney.Amount * cacheEntry.ExchangeRate);
    }

    public async Task<IEnumerable<CurrencyPair>> GetCurrencyPairsAsync(
        DateTimeOffset asOn,
        CancellationToken cancellationToken)
    {
        var cacheKey = Tuple.Create(MemoryCachedCurrencyConverterEntryType,
            MemoryCachedCurrencyConverterEntryKind.CurrencyPairs, this.instanceID, this.GetCacheIntervalKey(asOn));

        var cacheEntry = await this.GetFromMemoryCacheAsync(cacheKey,
            () => this.GetOriginalCurrencyPairsAsync(asOn, cancellationToken)).ConfigureAwait(false);

        return cacheEntry.CurrencyPairs;
    }

    public async Task<decimal> GetExchangeRateAsync(
        CurrencyPair pair,
        DateTimeOffset asOn,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(pair);

        var cacheKey = Tuple.Create(MemoryCachedCurrencyConverterEntryType,
            MemoryCachedCurrencyConverterEntryKind.ExchangeRate, this.instanceID, this.GetCacheIntervalKey(asOn),
            pair.BaseCurrency, pair.CounterCurrency);

        var cacheEntry = await this.GetFromMemoryCacheAsync(cacheKey,
            () => this.GetOriginalExchangeRateAsync(pair, asOn, cancellationToken)).ConfigureAwait(false);

        return cacheEntry.ExchangeRate;
    }

    [LoggerMessage(
        EventId = 469189242,
        Level = LogLevel.Warning,
        Message = "CacheInterval is 0, which makes caching redundant.")]
    private static partial void CacheIntervalIsZero(
        ILogger logger);

    private long GetCacheIntervalKey(DateTimeOffset asOn)
    {
        if (this.options.Value.CacheInterval.Ticks == 0L)
        {
            CacheIntervalIsZero(this.logger);

            return 0L;
        }

        return asOn.Ticks / this.options.Value.CacheInterval.Ticks;
    }

    private async Task<MemoryCachedCurrencyConverterEntry> GetOriginalCurrencyPairsAsync(
        DateTimeOffset asOn,
        CancellationToken cancellationToken)
    {
        var currencyPairs = await this.originalConverter.GetCurrencyPairsAsync(asOn, cancellationToken).ConfigureAwait(false);

        return MemoryCachedCurrencyConverterEntry.CreateForCurrencyPairs(currencyPairs);
    }

    private async Task<MemoryCachedCurrencyConverterEntry> GetOriginalExchangeRateAsync(
        CurrencyPair pair,
        DateTimeOffset asOn, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(pair);

        var exchangeRate = await this.originalConverter.GetExchangeRateAsync(pair, asOn, cancellationToken).ConfigureAwait(false);

        return MemoryCachedCurrencyConverterEntry.CreateForExchangeRate(exchangeRate);
    }

    private async Task<MemoryCachedCurrencyConverterEntry> OriginalConvertCurrencyAsync(
        Money baseMoney,
        CurrencyInfo counterCurrency,
        DateTimeOffset asOn,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(baseMoney);
        ArgumentNullException.ThrowIfNull(counterCurrency);

        var convertedMoney =
            await this.originalConverter.ConvertCurrencyAsync(baseMoney, counterCurrency, asOn, cancellationToken).ConfigureAwait(false);

        var exchangeRate = convertedMoney.Amount / baseMoney.Amount;

        return MemoryCachedCurrencyConverterEntry.CreateForExchangeRate(exchangeRate);
    }
}
