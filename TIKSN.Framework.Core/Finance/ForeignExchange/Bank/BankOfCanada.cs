using System.Diagnostics;
using System.Globalization;
using System.Text.Json;
using TIKSN.Globalization;

namespace TIKSN.Finance.ForeignExchange.Bank;

public class BankOfCanada : IBankOfCanada
{
    private static readonly TimeZoneInfo BankTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
    private static readonly CurrencyInfo CanadianDollar = new(new RegionInfo("en-CA"));

    private static readonly Dictionary<(Uri RestUrl, DateOnly RequestDate), List<Tuple<string, DateOnly, decimal>>>
        RawDataCache = [];

    private static readonly SemaphoreSlim RawDataSemaphore = new(initialCount: 1, maxCount: 1);

    private static readonly Uri RestURL =
        new("https://www.bankofcanada.ca/valet/observations/group/FX_RATES_DAILY/json");

    private readonly ICurrencyFactory currencyFactory;
    private readonly ICurrencyPairFactory currencyPairFactory;
    private readonly HttpClient httpClient;
    private readonly Dictionary<DateOnly, Dictionary<CurrencyInfo, decimal>> rates;
    private readonly TimeProvider timeProvider;
    private DateTimeOffset lastFetchDate;

    public BankOfCanada(
        HttpClient httpClient,
        ICurrencyFactory currencyFactory,
        ICurrencyPairFactory currencyPairFactory,
        TimeProvider timeProvider)
    {
        this.rates = [];

        this.lastFetchDate = DateTimeOffset.MinValue;
        this.httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        this.currencyFactory = currencyFactory ?? throw new ArgumentNullException(nameof(currencyFactory));
        this.currencyPairFactory = currencyPairFactory ?? throw new ArgumentNullException(nameof(currencyPairFactory));
        this.timeProvider = timeProvider ?? throw new ArgumentNullException(nameof(timeProvider));
    }

    public async Task<Money> ConvertCurrencyAsync(
        Money baseMoney,
        CurrencyInfo counterCurrency,
        DateTimeOffset asOn, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(baseMoney);
        ArgumentNullException.ThrowIfNull(counterCurrency);

        var pair = this.currencyPairFactory.Create(baseMoney.Currency, counterCurrency);

        var rate = await this.GetExchangeRateAsync(pair, asOn, cancellationToken).ConfigureAwait(false);

        return new Money(counterCurrency, baseMoney.Amount * rate);
    }

    public async Task<IReadOnlyCollection<CurrencyPair>> GetCurrencyPairsAsync(
        DateTimeOffset asOn,
        CancellationToken cancellationToken)
    {
        await this.FetchOnDemandAsync(cancellationToken).ConfigureAwait(false);

        if (asOn > this.timeProvider.GetUtcNow())
        {
            throw new ArgumentException("Exchange rate forecasting are not supported.", nameof(asOn));
        }

        var result = new List<CurrencyPair>();

        foreach (var against in this.GetRatesByDate(asOn).Keys)
        {
            result.Add(this.currencyPairFactory.Create(CanadianDollar, against));
            result.Add(this.currencyPairFactory.Create(against, CanadianDollar));
        }

        return result;
    }

    public async Task<decimal> GetExchangeRateAsync(
        CurrencyPair pair,
        DateTimeOffset asOn,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(pair);

        await this.FetchOnDemandAsync(cancellationToken).ConfigureAwait(false);

        if (asOn > this.timeProvider.GetUtcNow())
        {
            throw new ArgumentException("Exchange rate forecasting not supported.", nameof(asOn));
        }

        if (this.IsHomeCurrencyPair(pair, asOn))
        {
            return decimal.One / this.GetRatesByDate(asOn)[pair.CounterCurrency];
        }

        return this.GetRatesByDate(asOn)[pair.BaseCurrency];
    }

    public async Task<IReadOnlyCollection<ExchangeRate>> GetExchangeRatesAsync(
        DateTimeOffset asOn,
        CancellationToken cancellationToken)
    {
        var result = new List<ExchangeRate>();

        var ratesList = new List<Tuple<CurrencyInfo, DateOnly, decimal>>();

        var rawData = await this.FetchRawDataAsync(RestURL, cancellationToken).ConfigureAwait(false);

        var asOnDate = GetRateDate(asOn);
        foreach (var rawItem in rawData)
        {
            var currency = this.currencyFactory.Create(rawItem.Item1);

            if (asOnDate == rawItem.Item2)
            {
                result.Add(new ExchangeRate(this.currencyPairFactory.Create(currency, CanadianDollar),
                    GetRateDateTimeOffset(rawItem.Item2), rawItem.Item3));
            }

            ratesList.Add(new Tuple<CurrencyInfo, DateOnly, decimal>(currency, rawItem.Item2, rawItem.Item3));
        }

        lock (this.rates)
        {
            this.rates.Clear();

            foreach (var perDate in ratesList.GroupBy(item => item.Item2))
            {
                this.rates.Add(perDate.Key, perDate.ToDictionary(k => k.Item1, v => v.Item3));
            }
        }

        this.lastFetchDate = this.timeProvider.GetUtcNow(); // must stay at the end

        return result;
    }

    private static DateTimeOffset ConvertToBankTimeZone(DateTimeOffset date) =>
        TimeZoneInfo.ConvertTime(date, BankTimeZone);

    private static DateOnly GetRateDate(DateTimeOffset asOn)
        => DateOnly.FromDateTime(ConvertToBankTimeZone(asOn).Date);

    private static DateTimeOffset GetRateDateTimeOffset(DateOnly asOnDate)
    {
        var dateTime = asOnDate.ToDateTime(TimeOnly.MinValue);
        return new DateTimeOffset(dateTime, BankTimeZone.GetUtcOffset(dateTime));
    }

    private async Task FetchOnDemandAsync(CancellationToken cancellationToken)
    {
        if (this.timeProvider.GetUtcNow() - this.lastFetchDate > TimeSpan.FromDays(1d))
        {
            _ = await this.GetExchangeRatesAsync(this.timeProvider.GetUtcNow(), cancellationToken)
                .ConfigureAwait(false);
        }
    }

    private async Task<List<Tuple<string, DateOnly, decimal>>> FetchRawDataAsync(
        Uri restUrl,
        CancellationToken cancellationToken)
    {
        var cacheKey = (restUrl, GetRateDate(this.timeProvider.GetUtcNow()));

        await RawDataSemaphore.WaitAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            if (RawDataCache.TryGetValue(cacheKey, out var cachedRawData))
            {
                return cachedRawData;
            }

            var rawData = await this.FetchRawDataCoreAsync(restUrl, cancellationToken).ConfigureAwait(false);
            RawDataCache[cacheKey] = rawData;

            return rawData;
        }
        finally
        {
            _ = RawDataSemaphore.Release();
        }
    }

    private async Task<List<Tuple<string, DateOnly, decimal>>> FetchRawDataCoreAsync(
        Uri restUrl,
        CancellationToken cancellationToken)
    {
        var responseStream = await this.httpClient.GetStreamAsync(restUrl, cancellationToken).ConfigureAwait(false);

        using var streamReader = new StreamReader(responseStream);
        var root = JsonSerializer.Deserialize<JsonElement>(
            await streamReader.ReadToEndAsync(cancellationToken).ConfigureAwait(false));

        var result = new List<Tuple<string, DateOnly, decimal>>();

        var observationsElement = root.GetProperty("observations");

        foreach (var observation in observationsElement.EnumerateArray())
        {
            var d = observation.GetProperty("d");
            var asOn = DateOnly.ParseExact(d.GetString() ?? string.Empty, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            foreach (var fxElement in observation.EnumerateObject()
                         .Where(x => x.Name.StartsWith("FX", StringComparison.OrdinalIgnoreCase)))
            {
                Debug.Assert(fxElement.Name.EndsWith("CAD", StringComparison.Ordinal));
                var targetCurrencyCode = fxElement.Name.Substring(startIndex: 2, length: 3);
                var v = fxElement.Value.GetProperty("v");
                var rate = decimal.Parse(v.GetString() ?? string.Empty, CultureInfo.InvariantCulture);
                result.Add(new Tuple<string, DateOnly, decimal>(targetCurrencyCode, asOn, rate));
            }
        }

        return result;
    }

    private Dictionary<CurrencyInfo, decimal> GetRatesByDate(DateTimeOffset asOn)
    {
        var date = GetRateDate(asOn);

        if (this.rates.TryGetValue(date, out var valueAtDate))
        {
            return valueAtDate;
        }

        if (date.DayOfWeek == DayOfWeek.Saturday && this.rates.TryGetValue(date.AddDays(-1), out var valueAtYesterday))
        {
            return valueAtYesterday;
        }

        if (date.DayOfWeek == DayOfWeek.Sunday && this.rates.TryGetValue(date.AddDays(-2), out var valueAtTwoDaysAgo))
        {
            return valueAtTwoDaysAgo;
        }

        if (date.DayOfWeek == DayOfWeek.Monday && this.rates.TryGetValue(date.AddDays(-3), out var valueAtThreeDaysAgo))
        {
            return valueAtThreeDaysAgo;
        }

        if (this.rates.TryGetValue(date.AddDays(-1), out var otherwiseValueAtYesterday))
        {
            return otherwiseValueAtYesterday;
        }

        return this.rates[date]; // Exception will be thrown
    }

    private bool IsHomeCurrencyPair(CurrencyPair pair, DateTimeOffset asOn)
    {
        if (pair.BaseCurrency == CanadianDollar)
        {
            if (this.GetRatesByDate(asOn).Any(r => r.Key == pair.CounterCurrency))
            {
                return true;
            }
        }
        else if (pair.CounterCurrency == CanadianDollar &&
                 this.GetRatesByDate(asOn).Any(r => r.Key == pair.BaseCurrency))
        {
            return false;
        }

        throw new ArgumentException("Currency pair not supported.", nameof(pair));
    }
}
