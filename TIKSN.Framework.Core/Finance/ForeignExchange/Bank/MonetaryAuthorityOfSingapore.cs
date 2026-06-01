using System.Globalization;
using System.Text.Json;
using NodaTime;
using NodaTime.Extensions;
using TIKSN.Globalization;

namespace TIKSN.Finance.ForeignExchange.Bank;

public class MonetaryAuthorityOfSingapore : IMonetaryAuthorityOfSingapore
{
    private const string DataSeriesPropertyName = "DataSeries";
    private const string IdPropertyName = "_id";

    private static readonly Uri DataStoreUrl =
        new("https://data.gov.sg/api/action/datastore_search?resource_id=d_cdd73fd4341b345fa4307e44d6f82175&limit=100");

    /// <summary>
    /// Units is the foreign-currency quantity represented by each published SGD rate.
    /// </summary>
    private static readonly Dictionary<string, RateSeries> RateSeriesMap =
        new(StringComparer.InvariantCulture)
        {
            ["Australian Dollar"] = new("AUD", Units: 1m),
            ["Euro"] = new("EUR", Units: 1m),
            ["Hong Kong Dollar"] = new("HKD", Units: 1m),
            ["Indian Rupee"] = new("INR", Units: 100m),
            ["Indonesian Rupiah"] = new("IDR", Units: 100m),
            ["Japanese Yen"] = new("JPY", Units: 100m),
            ["Korean Won"] = new("KRW", Units: 100m),
            ["Malaysian Ringgit"] = new("MYR", Units: 1m),
            ["New Taiwan Dollar"] = new("TWD", Units: 100m),
            ["Philippine Peso"] = new("PHP", Units: 100m),
            ["Renminbi"] = new("CNY", Units: 1m),
            ["Sterling Pound"] = new("GBP", Units: 1m),
            ["Swiss Franc"] = new("CHF", Units: 1m),
            ["Thai Baht"] = new("THB", Units: 100m),
            ["US Dollar"] = new("USD", Units: 1m),
        };

    private static readonly CurrencyInfo SingaporeDollar = new(new RegionInfo("en-SG"));
    private static readonly DateTimeZone SingaporeTimeZone = DateTimeZoneProviders.Tzdb["Asia/Singapore"];

    private readonly ICurrencyFactory currencyFactory;
    private readonly HttpClient httpClient;
    private readonly TimeProvider timeProvider;
    private IReadOnlyCollection<ExchangeRate>? cachedRates;
    private DateTimeOffset lastFetchDate;

    public MonetaryAuthorityOfSingapore(
        HttpClient httpClient,
        ICurrencyFactory currencyFactory,
        TimeProvider timeProvider)
    {
        this.httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        this.currencyFactory = currencyFactory ?? throw new ArgumentNullException(nameof(currencyFactory));
        this.timeProvider = timeProvider ?? throw new ArgumentNullException(nameof(timeProvider));
        this.lastFetchDate = DateTimeOffset.MinValue;
    }

    public async Task<Money> ConvertCurrencyAsync(
        Money baseMoney,
        CurrencyInfo counterCurrency,
        DateTimeOffset asOn,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(baseMoney);
        ArgumentNullException.ThrowIfNull(counterCurrency);

        var pair = new CurrencyPair(baseMoney.Currency, counterCurrency);
        var rate = await this.GetExchangeRateAsync(pair, asOn, cancellationToken).ConfigureAwait(false);

        return new Money(counterCurrency, baseMoney.Amount * rate);
    }

    public async Task<IReadOnlyCollection<CurrencyPair>> GetCurrencyPairsAsync(
        DateTimeOffset asOn,
        CancellationToken cancellationToken)
    {
        var rates = await this.GetRatesAsync(asOn, cancellationToken).ConfigureAwait(false);

        return [.. rates.Select(x => x.Pair).Distinct()];
    }

    public async Task<decimal> GetExchangeRateAsync(
        CurrencyPair pair,
        DateTimeOffset asOn,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(pair);

        var rates = await this.GetRatesAsync(asOn, cancellationToken).ConfigureAwait(false);
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
        => await this.GetRatesAsync(asOn, cancellationToken).ConfigureAwait(false);

    private static DateTimeOffset CreateSingaporeDateTimeOffset(DateOnly date)
    {
        var localDate = new LocalDate(date.Year, date.Month, date.Day);

        return localDate.AtStartOfDayInZone(SingaporeTimeZone).ToDateTimeOffset();
    }

    private static DateTimeOffset CreateSingaporeMonthEndDateTimeOffset(int year, int month)
    {
        var day = DateTime.DaysInMonth(year, month);

        return CreateSingaporeDateTimeOffset(new DateOnly(year, month, day));
    }

    private static DateOnly GetSingaporeDate(DateTimeOffset dateTime) =>
        DateOnly.FromDateTime(dateTime.ToInstant().InZone(SingaporeTimeZone).ToDateTimeUnspecified());

    private static bool TryGetRate(JsonElement value, out decimal rate)
    {
        if (value.ValueKind == JsonValueKind.Number)
        {
            return value.TryGetDecimal(out rate);
        }

        if (value.ValueKind == JsonValueKind.String)
        {
            return decimal.TryParse(
                value.GetString(),
                NumberStyles.Number,
                CultureInfo.InvariantCulture,
                out rate);
        }

        rate = decimal.Zero;
        return false;
    }

    private static bool TryParsePeriod(string fieldName, out DateTimeOffset asOn)
    {
        if (DateTime.TryParseExact(
                fieldName,
                "yyyyMMM",
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out var period))
        {
            asOn = CreateSingaporeMonthEndDateTimeOffset(period.Year, period.Month);
            return true;
        }

        asOn = DateTimeOffset.MinValue;
        return false;
    }

    private void AddExchangeRates(
        List<ExchangeRate> rates,
        RateSeries series,
        JsonProperty property)
    {
        if (!TryParsePeriod(property.Name, out var asOn) ||
            !TryGetRate(property.Value, out var rawRate) ||
            rawRate <= decimal.Zero)
        {
            return;
        }

        var currency = this.currencyFactory.Create(series.CurrencyCode);
        var rate = rawRate / series.Units;
        var exchangeRate = new ExchangeRate(new CurrencyPair(currency, SingaporeDollar), asOn, rate);

        rates.Add(exchangeRate);
        rates.Add(exchangeRate.Reverse());
    }

    private async Task<IReadOnlyCollection<ExchangeRate>> FetchRatesAsync(CancellationToken cancellationToken)
    {
        var responseStream =
            await this.httpClient.GetStreamAsync(DataStoreUrl, cancellationToken).ConfigureAwait(false);

        await using (responseStream.ConfigureAwait(false))
        {
            var root = await JsonSerializer
                .DeserializeAsync<JsonElement>(responseStream, cancellationToken: cancellationToken)
                .ConfigureAwait(false);

            var rates = new List<ExchangeRate>();

            foreach (var record in root.GetProperty("result").GetProperty("records").EnumerateArray())
            {
                var seriesName = record.GetProperty(DataSeriesPropertyName).GetString();

                if (seriesName is null || !RateSeriesMap.TryGetValue(seriesName, out var series))
                {
                    continue;
                }

                foreach (var property in record.EnumerateObject())
                {
                    if (string.Equals(property.Name, DataSeriesPropertyName, StringComparison.Ordinal) ||
                        string.Equals(property.Name, IdPropertyName, StringComparison.Ordinal))
                    {
                        continue;
                    }

                    this.AddExchangeRates(rates, series, property);
                }
            }

            this.lastFetchDate = this.timeProvider.GetUtcNow();
            this.cachedRates = rates;

            return rates;
        }
    }

    private async Task<IReadOnlyCollection<ExchangeRate>> GetAllRatesAsync(CancellationToken cancellationToken)
    {
        if (this.cachedRates is null || this.timeProvider.GetUtcNow() - this.lastFetchDate > TimeSpan.FromDays(1d))
        {
            return await this.FetchRatesAsync(cancellationToken).ConfigureAwait(false);
        }

        return this.cachedRates;
    }

    private async Task<IReadOnlyCollection<ExchangeRate>> GetRatesAsync(
        DateTimeOffset asOn,
        CancellationToken cancellationToken)
    {
        if (asOn > this.timeProvider.GetUtcNow())
        {
            throw new ArgumentException("Exchange rate forecasting are not supported.", nameof(asOn));
        }

        var allRates = await this.GetAllRatesAsync(cancellationToken).ConfigureAwait(false);
        var singaporeDate = GetSingaporeDate(asOn);
        var latestDate = allRates
            .Where(x => GetSingaporeDate(x.AsOn) <= singaporeDate)
            .Select(x => x.AsOn)
            .Distinct()
            .OrderByDescending(x => x)
            .FirstOrDefault();

        if (latestDate == DateTimeOffset.MinValue)
        {
            throw new ArgumentException("Exchange rate history not supported.", nameof(asOn));
        }

        return [.. allRates.Where(x => x.AsOn == latestDate).Distinct()];
    }

    private readonly record struct RateSeries(string CurrencyCode, decimal Units);
}
