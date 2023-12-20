namespace TIKSN.Finance.Cache;

public class CachedCurrencyConverter : ICurrencyConverter
{
    private readonly List<CachedCurrencyPairs> cachedCurrencyPairs;
    private readonly List<CachedRate> cachedRates;
    private readonly ICurrencyConverter originalConverter;
    private readonly TimeProvider timeProvider;

    public CachedCurrencyConverter(ICurrencyConverter originalConverter, TimeProvider timeProvider,
        TimeSpan ratesCacheInterval, TimeSpan currencyPairsCacheInterval, int? ratesCacheCapacity = null,
        int? currencyPairsCacheCapacity = null)
    {
        if (ratesCacheCapacity.HasValue)
        {
            if (ratesCacheCapacity.Value < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(ratesCacheCapacity),
                    "Rates cache capacity can not be negative.");
            }
        }

        if (currencyPairsCacheCapacity.HasValue)
        {
            if (currencyPairsCacheCapacity.Value < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(currencyPairsCacheCapacity),
                    "Currency pairs cache capacity can not be negative.");
            }
        }

        if (ratesCacheInterval < TimeSpan.Zero)
        {
            throw new ArgumentOutOfRangeException(nameof(ratesCacheInterval),
                "Rates cache interval can not be negative.");
        }

        if (currencyPairsCacheInterval < TimeSpan.Zero)
        {
            throw new ArgumentOutOfRangeException(nameof(currencyPairsCacheInterval),
                "Currency pairs cache interval can not be negative.");
        }

        this.originalConverter = originalConverter ?? throw new ArgumentNullException(nameof(originalConverter));
        this.RatesCacheInterval = ratesCacheInterval;
        this.CurrencyPairsCacheInterval = currencyPairsCacheInterval;
        this.RatesCacheCapacity = ratesCacheCapacity;
        this.CurrencyPairsCacheCapacity = currencyPairsCacheCapacity;

        this.cachedCurrencyPairs = [];
        this.cachedRates = [];
        this.timeProvider = timeProvider ?? throw new ArgumentNullException(nameof(timeProvider));
    }

    public int? CurrencyPairsCacheCapacity { get; }

    public TimeSpan CurrencyPairsCacheInterval { get; }

    public int CurrencyPairsCacheSize => this.cachedCurrencyPairs.Count;

    public int? RatesCacheCapacity { get; }

    public TimeSpan RatesCacheInterval { get; }

    public int RatesCacheSize => this.cachedRates.Count;

    public void Clear()
    {
        this.cachedCurrencyPairs.Clear();
        this.cachedRates.Clear();
    }

    public async Task<Money> ConvertCurrencyAsync(
        Money baseMoney,
        CurrencyInfo counterCurrency,
        DateTimeOffset asOn,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(baseMoney);
        ArgumentNullException.ThrowIfNull(counterCurrency);

        if (baseMoney.Amount == decimal.Zero)
        {
            return new Money(counterCurrency);
        }

        var pair = new CurrencyPair(baseMoney.Currency, counterCurrency);

        var cachedRate = this.GetFromCache(this.cachedRates.Where(item => item.Pair == pair),
            this.RatesCacheInterval, asOn);

        if (cachedRate != null)
        {
            return new Money(counterCurrency, baseMoney.Amount * cachedRate.ExchangeRate);
        }

        var actualMoney =
            await this.originalConverter.ConvertCurrencyAsync(baseMoney, counterCurrency, asOn, cancellationToken).ConfigureAwait(false);

        var actualRate = actualMoney.Amount / baseMoney.Amount;

        AddToCache(this.cachedRates, this.RatesCacheCapacity,
            new CachedRate(pair, actualRate, asOn, this.timeProvider));

        return actualMoney;
    }

    public async Task<IEnumerable<CurrencyPair>> GetCurrencyPairsAsync(
        DateTimeOffset asOn,
        CancellationToken cancellationToken)
    {
        var cachedPairs = this.GetFromCache(this.cachedCurrencyPairs, this.CurrencyPairsCacheInterval, asOn);

        if (cachedPairs != null)
        {
            return cachedPairs.CurrencyPairs;
        }

        var actualPairs = await this.originalConverter.GetCurrencyPairsAsync(asOn, cancellationToken).ConfigureAwait(false);

        AddToCache(this.cachedCurrencyPairs, this.CurrencyPairsCacheCapacity,
            new CachedCurrencyPairs(actualPairs, asOn, this.timeProvider));

        return actualPairs;
    }

    public async Task<decimal> GetExchangeRateAsync(
        CurrencyPair pair,
        DateTimeOffset asOn,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(pair);

        var cachedRate = this.GetFromCache(this.cachedRates.Where(item => item.Pair == pair),
            this.RatesCacheInterval, asOn);

        if (cachedRate != null)
        {
            return cachedRate.ExchangeRate;
        }

        var actualRate = await this.originalConverter.GetExchangeRateAsync(pair, asOn, cancellationToken).ConfigureAwait(false);

        AddToCache(this.cachedRates, this.RatesCacheCapacity,
            new CachedRate(pair, actualRate, asOn, this.timeProvider));

        return actualRate;
    }

    private static TimeSpan Absolute(TimeSpan value)
    {
        if (value < TimeSpan.Zero)
        {
            return -value;
        }

        return value;
    }

    private static void AddToCache<T>(List<T> cache, int? capacity, T itemToCache) where T : CachedData
    {
        lock (cache)
        {
            if (capacity != 0)
            {
                if (capacity.HasValue)
                {
                    if (cache.Count + 1 > capacity.Value)
                    {
                        var itemToRemove = cache.OrderBy(item => item.LastAccess).First();

                        _ = cache.Remove(itemToRemove);
                    }

                    cache.Add(itemToCache);
                }
                else
                {
                    cache.Add(itemToCache);
                }
            }
        }
    }

    private static bool IsActual(DateTimeOffset cachedAsOn, DateTimeOffset actualAsOn, TimeSpan interval)
    {
        if (cachedAsOn < actualAsOn)
        {
            if (actualAsOn - cachedAsOn <= interval)
            {
                return true;
            }
        }
        else
        {
            if (cachedAsOn - actualAsOn < interval)
            {
                return true;
            }
        }

        return false;
    }

    private T GetFromCache<T>(IEnumerable<T> cache, TimeSpan interval, DateTimeOffset asOn) where T : CachedData
    {
        var cachedItem = cache.Where(item => IsActual(item.AsOn, asOn, interval))
            .OrderBy(item => Absolute(item.AsOn - asOn)).FirstOrDefault();

        cachedItem?.Update(this.timeProvider);

        return cachedItem;
    }

    private class CachedCurrencyPairs : CachedData
    {
        public CachedCurrencyPairs(IEnumerable<CurrencyPair> currencyPairs, DateTimeOffset asOn,
            TimeProvider timeProvider)
        {
            this.CurrencyPairs = currencyPairs;
            this.AsOn = asOn;

            this.Update(timeProvider);
        }

        public IEnumerable<CurrencyPair> CurrencyPairs { get; }
    }

    private abstract class CachedData
    {
        public DateTimeOffset AsOn { get; set; }

        public DateTimeOffset LastAccess { get; private set; }

        public void Update(TimeProvider timeProvider) => this.LastAccess = timeProvider.GetUtcNow();
    }

    private class CachedRate : CachedData
    {
        public CachedRate(CurrencyPair pair, decimal exchangeRate, DateTimeOffset asOn, TimeProvider timeProvider)
        {
            this.Pair = pair;
            this.ExchangeRate = exchangeRate;
            this.AsOn = asOn;

            this.Update(timeProvider);
        }

        public decimal ExchangeRate { get; }

        public CurrencyPair Pair { get; }
    }
}
