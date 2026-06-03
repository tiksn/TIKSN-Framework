using System.Globalization;
using System.Text;
using System.Text.Json;
using NodaTime;
using NodaTime.Extensions;
using TIKSN.Globalization;

namespace TIKSN.Finance.ForeignExchange.Bank;

public class BankOfJapan : IBankOfJapan
{
    private const string EuroCurrencyCode = "EUR";

    private const string EuroUsDollar17JstSeriesCode = "FXERD34";
    private const string JapaneseYenCurrencyCode = "JPY";
    private const string UsDollarCurrencyCode = "USD";
    private const string UsDollarYen17JstSeriesCode = "FXERD04";

    private static readonly IReadOnlyList<RawSeriesDefinition> RawSeriesDefinitions =
    [
        new(UsDollarCurrencyCode, JapaneseYenCurrencyCode, UsDollarYen17JstSeriesCode),
        new(EuroCurrencyCode, UsDollarCurrencyCode, EuroUsDollar17JstSeriesCode),
    ];

    private static readonly Dictionary<Uri, IReadOnlyCollection<RawExchangeRate>> RawRatesCache = [];
    private static readonly SemaphoreSlim RawRatesSemaphore = new(initialCount: 1, maxCount: 1);


#pragma warning disable S1075
    private static readonly CompositeFormat RequestUrlCompositeString = CompositeFormat.Parse(
        "https://www.stat-search.boj.or.jp/api/v1/getDataCode?format=json&lang=en&db=FM08&startDate={0}&endDate={0}&code={1}");
#pragma warning restore S1075

    private static readonly DateTimeZone TokyoTimeZone = DateTimeZoneProviders.Tzdb["Asia/Tokyo"];

    private readonly ICurrencyFactory currencyFactory;
    private readonly ICurrencyPairFactory currencyPairFactory;
    private readonly HttpClient httpClient;
    private readonly Dictionary<string, SeriesDefinition> seriesDefinitions;
    private readonly TimeProvider timeProvider;

    public BankOfJapan(
        HttpClient httpClient,
        ICurrencyFactory currencyFactory,
        ICurrencyPairFactory currencyPairFactory,
        TimeProvider timeProvider)
    {
        this.httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        this.currencyFactory = currencyFactory ?? throw new ArgumentNullException(nameof(currencyFactory));
        this.currencyPairFactory = currencyPairFactory ?? throw new ArgumentNullException(nameof(currencyPairFactory));
        this.seriesDefinitions = CreateSeriesDefinitions(this.currencyFactory, this.currencyPairFactory);
        this.timeProvider = timeProvider ?? throw new ArgumentNullException(nameof(timeProvider));
    }

    public async Task<Money> ConvertCurrencyAsync(
        Money baseMoney,
        CurrencyInfo counterCurrency,
        DateTimeOffset asOn,
        CancellationToken cancellationToken)
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
        var rates = await this.GetExchangeRatesAsync(asOn, cancellationToken).ConfigureAwait(false);

        return [.. rates.Select(x => x.Pair).Distinct()];
    }

    public async Task<decimal> GetExchangeRateAsync(
        CurrencyPair pair,
        DateTimeOffset asOn,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(pair);

        var rates = await this.GetExchangeRatesAsync(asOn, cancellationToken).ConfigureAwait(false);
        var exchangeRate = rates.SingleOrDefault(x => x.Pair == pair);

        if (exchangeRate is not null)
        {
            return exchangeRate.Rate;
        }

        throw new ArgumentException($"Currency pair '{pair}' not supported.", nameof(pair));
    }

    public async Task<IReadOnlyCollection<ExchangeRate>> GetExchangeRatesAsync(
        DateTimeOffset asOn,
        CancellationToken cancellationToken)
    {
        if (asOn > this.timeProvider.GetUtcNow())
        {
            throw new ArgumentException("Exchange rate forecasting not supported.", nameof(asOn));
        }

        var rateDate = GetTokyoDate(asOn);
        var rawRates = await this.GetRawRatesAsync(rateDate, cancellationToken).ConfigureAwait(false);

        return this.CreateExchangeRates(rawRates);
    }

    private static Uri CreateRequestUrl(DateOnly month)
    {
        var period = month.ToString("yyyyMM", CultureInfo.InvariantCulture);
        var requestUrl = string.Format(
            CultureInfo.InvariantCulture,
            RequestUrlCompositeString,
            period,
            string.Join(separator: ',', RawSeriesDefinitions.Select(x => x.SeriesCode)));

        return new Uri(requestUrl);
    }

    private static Dictionary<string, SeriesDefinition> CreateSeriesDefinitions(
        ICurrencyFactory currencyFactory,
        ICurrencyPairFactory currencyPairFactory)
        => RawSeriesDefinitions.ToDictionary(
            x => x.SeriesCode,
            x => new SeriesDefinition(
                currencyPairFactory.Create(
                    currencyFactory.Create(x.BaseCurrencyCode),
                    currencyFactory.Create(x.CounterCurrencyCode))),
            StringComparer.Ordinal);

    private static DateTimeOffset CreateTokyoDateTimeOffset(DateOnly date)
    {
        var localDate = new LocalDate(date.Year, date.Month, date.Day);

        return localDate.AtStartOfDayInZone(TokyoTimeZone).ToDateTimeOffset();
    }

    private static IReadOnlyCollection<RawExchangeRate> GetRawRatesForLatestDate(
        IEnumerable<RawExchangeRate> rawRates,
        DateOnly rateDate)
    {
        var latestDate = rawRates
            .Where(x => x.AsOn <= rateDate)
            .GroupBy(x => x.AsOn)
            .Where(x => x.Select(r => r.SeriesCode).Distinct().Count() == RawSeriesDefinitions.Count)
            .Select(x => x.Key)
            .OrderByDescending(x => x)
            .FirstOrDefault();

        if (latestDate == default)
        {
            return [];
        }

        return [.. rawRates.Where(x => x.AsOn == latestDate)];
    }

    private static DateOnly GetTokyoDate(DateTimeOffset dateTime) =>
        DateOnly.FromDateTime(dateTime.ToInstant().InZone(TokyoTimeZone).ToDateTimeUnspecified());

    private static DateOnly ParseSurveyDate(int surveyDate)
        => new(
            surveyDate / 10000,
            surveyDate % 10000 / 100,
            surveyDate % 100);

    private void AddEuroYenCrossRate(
        List<ExchangeRate> rates,
        Dictionary<CurrencyPair, decimal> directRates,
        DateTimeOffset asOn)
    {
        var euro = this.currencyFactory.Create(EuroCurrencyCode);
        var japaneseYen = this.currencyFactory.Create(JapaneseYenCurrencyCode);
        var usDollar = this.currencyFactory.Create(UsDollarCurrencyCode);
        var euroUsDollar = this.currencyPairFactory.Create(euro, usDollar);
        var usDollarYen = this.currencyPairFactory.Create(usDollar, japaneseYen);
        var euroYen = this.currencyPairFactory.Create(euro, japaneseYen);

        this.AddRateAndReverse(
            rates,
            euroYen,
            asOn,
            directRates[euroUsDollar] * directRates[usDollarYen]);
    }

    private void AddRateAndReverse(
        List<ExchangeRate> rates,
        CurrencyPair pair,
        DateTimeOffset asOn,
        decimal rate)
    {
        rates.Add(new ExchangeRate(pair, asOn, rate));
        rates.Add(new ExchangeRate(this.currencyPairFactory.Reverse(pair), asOn, decimal.One / rate));
    }

    private List<ExchangeRate> CreateExchangeRates(IReadOnlyCollection<RawExchangeRate> rawRates)
    {
        var rates = new List<ExchangeRate>();
        var directRates = new Dictionary<CurrencyPair, decimal>();
        var asOn = CreateTokyoDateTimeOffset(rawRates.First().AsOn);

        foreach (var rawRate in rawRates)
        {
            var pair = this.seriesDefinitions[rawRate.SeriesCode].Pair;

            directRates.Add(pair, rawRate.Rate);
            this.AddRateAndReverse(rates, pair, asOn, rawRate.Rate);
        }

        this.AddEuroYenCrossRate(rates, directRates, asOn);

        return rates;
    }

    private async Task<IReadOnlyCollection<RawExchangeRate>> FetchRawRatesAsync(
        DateOnly month,
        CancellationToken cancellationToken)
    {
        var requestUrl = CreateRequestUrl(month);

        await RawRatesSemaphore.WaitAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            if (!RawRatesCache.TryGetValue(requestUrl, out var rawRates))
            {
                rawRates = await this.FetchRawRatesCoreAsync(requestUrl, cancellationToken).ConfigureAwait(false);
                RawRatesCache[requestUrl] = rawRates;
            }

            return rawRates;
        }
        finally
        {
            _ = RawRatesSemaphore.Release();
        }
    }

    private async Task<IReadOnlyCollection<RawExchangeRate>> FetchRawRatesCoreAsync(
        Uri requestUrl,
        CancellationToken cancellationToken)
    {
        var responseStream = await this.httpClient.GetStreamAsync(requestUrl, cancellationToken).ConfigureAwait(false);

        await using (responseStream.ConfigureAwait(false))
        {
            var root = await JsonSerializer
                .DeserializeAsync<JsonElement>(responseStream, cancellationToken: cancellationToken)
                .ConfigureAwait(false);

            var result = new List<RawExchangeRate>();

            foreach (var series in root.GetProperty("RESULTSET").EnumerateArray())
            {
                var seriesCode = series.GetProperty("SERIES_CODE").GetString() ?? string.Empty;

                var dates = series.GetProperty("VALUES").GetProperty("SURVEY_DATES").EnumerateArray()
                    .Select(x => ParseSurveyDate(x.GetInt32()))
                    .ToArray();
                var values = series.GetProperty("VALUES").GetProperty("VALUES").EnumerateArray().ToArray();

                for (var index = 0; index < dates.Length; index++)
                {
                    if (values[index].ValueKind == JsonValueKind.Number &&
                        values[index].TryGetDecimal(out var value) &&
                        value > decimal.Zero)
                    {
                        result.Add(new RawExchangeRate(seriesCode, dates[index], value));
                    }
                }
            }

            return result;
        }
    }

    private async Task<IReadOnlyCollection<RawExchangeRate>> GetRawRatesAsync(
        DateOnly rateDate,
        CancellationToken cancellationToken)
    {
        var month = new DateOnly(rateDate.Year, rateDate.Month, day: 1);
        var rawRates = await this.FetchRawRatesAsync(month, cancellationToken).ConfigureAwait(false);
        var rates = GetRawRatesForLatestDate(rawRates, rateDate);

        if (rates.Count > 0)
        {
            return rates;
        }

        var previousMonth = month.AddMonths(-1);
        rawRates = await this.FetchRawRatesAsync(previousMonth, cancellationToken).ConfigureAwait(false);
        rates = GetRawRatesForLatestDate(rawRates, rateDate);

        if (rates.Count > 0)
        {
            return rates;
        }

        throw new ArgumentException("Exchange rate history not supported.", nameof(rateDate));
    }

    private sealed record RawExchangeRate(string SeriesCode, DateOnly AsOn, decimal Rate);

    private sealed record RawSeriesDefinition(string BaseCurrencyCode, string CounterCurrencyCode, string SeriesCode);

    private sealed record SeriesDefinition(CurrencyPair Pair);
}
