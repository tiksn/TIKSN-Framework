using System.Globalization;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using NodaTime;
using NodaTime.Extensions;
using TIKSN.Globalization;

namespace TIKSN.Finance.ForeignExchange.Bank;

/// <summary>
///     Exchange rate converter of National Bank of Poland
/// </summary>
/// <seealso cref="ICurrencyConverter" />
public class NationalBankOfPoland : INationalBankOfPoland
{
    private const string TableA = "a";
    private const string TableB = "b";

    private static readonly MediaTypeWithQualityHeaderValue AcceptHeaderValue = new("application/json");

    private static readonly RegionInfo Poland = new("pl-PL");
    private static readonly CultureInfo PolishCulture = new("pl-PL");
    private static readonly CurrencyInfo PolishZloty = new(Poland);
    private static readonly DateTimeZone WarsawTimeZone = DateTimeZoneProviders.Tzdb["Europe/Warsaw"];

    private static readonly CompositeFormat WebServiceUrlFormat =
        CompositeFormat.Parse("https://api.nbp.pl/api/exchangerates/tables/{0}/{1:yyyy-MM-dd}/{2:yyyy-MM-dd}");

    private readonly ICurrencyFactory currencyFactory;
    private readonly HttpClient httpClient;

    public NationalBankOfPoland(
        HttpClient httpClient,
        ICurrencyFactory currencyFactory)
    {
        this.httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        this.currencyFactory = currencyFactory ?? throw new ArgumentNullException(nameof(currencyFactory));
    }

    public async Task<Money> ConvertCurrencyAsync(
        Money baseMoney,
        CurrencyInfo counterCurrency,
        DateTimeOffset asOn,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(baseMoney);
        ArgumentNullException.ThrowIfNull(counterCurrency);

        var exchangeRates = await this.GetRatesAsync(asOn, cancellationToken).ConfigureAwait(false);

        var exchangeRate = exchangeRates
            .Where(x => x.Pair.BaseCurrency == baseMoney.Currency && x.Pair.CounterCurrency == counterCurrency)
            .MinByWithTies(x => (x.AsOn - asOn).Duration())[0];

        return new Money(counterCurrency, exchangeRate.Rate * baseMoney.Amount);
    }

    public async Task<IEnumerable<CurrencyPair>> GetCurrencyPairsAsync(
        DateTimeOffset asOn,
        CancellationToken cancellationToken)
    {
        var exchangeRates = await this.GetRatesAsync(asOn, cancellationToken).ConfigureAwait(false);

        return exchangeRates
            .Select(x => x.Pair)
            .Distinct();
    }

    public async Task<decimal> GetExchangeRateAsync(
        CurrencyPair pair,
        DateTimeOffset asOn,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(pair);

        var exchangeRates = await this.GetRatesAsync(asOn, cancellationToken).ConfigureAwait(false);

        var exchangeRate = exchangeRates
            .Where(x => x.Pair == pair)
            .MinByWithTies(x => (x.AsOn - asOn).Duration())[0];

        return exchangeRate.Rate;
    }

    public async Task<IEnumerable<ExchangeRate>> GetExchangeRatesAsync(
        DateTimeOffset asOn,
        CancellationToken cancellationToken)
        => await this.GetRatesAsync(asOn, cancellationToken).ConfigureAwait(false);

    private static IReadOnlyList<ValueTuple<DateTimeOffset, Rate>> CreateDatesAndRates(
        RateTable source)
    {
        ArgumentNullException.ThrowIfNull(source);

        var asOnWarsaw = source.EffectiveDate.ToLocalDate().AtStartOfDayInZone(WarsawTimeZone).ToDateTimeOffset();

        return source.Rates.Select(x => (asOnWarsaw, x)).ToArray();
    }

    private ExchangeRate CreateExchangeRate(ValueTuple<DateTimeOffset, Rate> entry)
    {
        var currency = this.currencyFactory.Create(entry.Item2.Code);
        var pair = new CurrencyPair(currency, PolishZloty);
        return new ExchangeRate(pair, entry.Item1, entry.Item2.Mid);
    }

    private async Task<IReadOnlyList<ExchangeRate>> GetRatesAsync(
        string table,
        DateTimeOffset asOn,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(table);

        var asOnWarsaw = asOn.ToInstant().InZone(WarsawTimeZone).LocalDateTime;
        var weekAgoWarsaw = asOnWarsaw.Plus(Period.FromDays(-7));

        var requestUrl = new Uri(string.Format(PolishCulture, WebServiceUrlFormat, table, weekAgoWarsaw, asOnWarsaw));

        using var request = new HttpRequestMessage
        {
            Method = HttpMethod.Get,
            RequestUri = requestUrl,
        };
        request.Headers.Accept.Add(AcceptHeaderValue);

        var response = await this.httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return [];
        }

        _ = response.EnsureSuccessStatusCode();

        var stream = await response.Content.ReadAsStreamAsync(cancellationToken).ConfigureAwait(false);
        var rateTables = await JsonSerializer.DeserializeAsync<RateTable[]>(stream, cancellationToken: cancellationToken).ConfigureAwait(false);

        return rateTables
            .SelectMany(CreateDatesAndRates)
            .Select(this.CreateExchangeRate)
            .ToArray();
    }

    private async Task<IReadOnlyList<ExchangeRate>> GetRatesAsync(
        DateTimeOffset asOn,
        CancellationToken cancellationToken)
    {
        var ratesA = await this.GetRatesAsync(TableA, asOn, cancellationToken).ConfigureAwait(false);
        var ratesB = await this.GetRatesAsync(TableB, asOn, cancellationToken).ConfigureAwait(false);

        return ratesA.Concat(ratesB)
            .SelectMany(x => new[] { x, new ExchangeRate(x.Pair.Reverse(), x.AsOn, 1m / x.Rate) })
            .ToArray();
    }

#pragma warning disable CA1812 // Avoid uninstantiated internal classes

    private sealed class Rate
#pragma warning restore CA1812 // Avoid uninstantiated internal classes
    {
        [JsonPropertyName("code")]
        public string Code { get; set; }

        [JsonPropertyName("mid")]
        public decimal Mid { get; set; }
    }

#pragma warning disable CA1812 // Avoid uninstantiated internal classes

    private sealed class RateTable
#pragma warning restore CA1812 // Avoid uninstantiated internal classes
    {
        [JsonPropertyName("effectiveDate")]
        public DateOnly EffectiveDate { get; set; }

        [JsonPropertyName("rates")]
        public List<Rate> Rates { get; set; }
    }
}
