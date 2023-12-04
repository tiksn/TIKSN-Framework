using System.Globalization;
using Newtonsoft.Json;
using TIKSN.Globalization;

namespace TIKSN.Finance.ForeignExchange.Cumulative;

public class CurrencylayerDotCom : ICurrencylayerDotCom
{
    private const string HistoricalBaseURL = "https://apilayer.net/api/historical?";
    private const string LiveBaseURL = "https://apilayer.net/api/live?";
    private readonly string accessKey;
    private readonly ICurrencyFactory currencyFactory;
    private readonly HttpClient httpClient;
    private readonly TimeProvider timeProvider;

    public CurrencylayerDotCom(
        HttpClient httpClient,
        ICurrencyFactory currencyFactory,
        TimeProvider timeProvider,
        string accessKey)
    {
        if (string.IsNullOrEmpty(accessKey))
        {
            throw new ArgumentNullException(nameof(accessKey));
        }

        this.accessKey = accessKey;
        this.httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        this.currencyFactory = currencyFactory ?? throw new ArgumentNullException(nameof(currencyFactory));
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

        var rates = await this.GetRatesAsync(baseMoney.Currency, counterCurrency, asOn, cancellationToken).ConfigureAwait(false);

        var rate = rates.Values.Single();

        return new Money(counterCurrency, baseMoney.Amount * rate);
    }

    public async Task<IEnumerable<CurrencyPair>> GetCurrencyPairsAsync(
        DateTimeOffset asOn,
        CancellationToken cancellationToken)
    {
        var pairsWithRates = await this.GetRatesAsync(baseCurrency: null, counterCurrency: null, asOn, cancellationToken).ConfigureAwait(false);

        var currencies = pairsWithRates.Keys.Select(item => item.CounterCurrency).ToSeq();

        var pairs = new List<CurrencyPair>();

        foreach (var currency1 in currencies)
        {
            foreach (var currency2 in currencies)
            {
                if (currency1 != currency2)
                {
                    pairs.Add(new CurrencyPair(currency1, currency2));
                }
            }
        }

        return pairs;
    }

    public async Task<decimal> GetExchangeRateAsync(
        CurrencyPair pair,
        DateTimeOffset asOn,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(pair);

        var rates = await this.GetRatesAsync(pair.BaseCurrency, pair.CounterCurrency, asOn, cancellationToken).ConfigureAwait(false);

        return rates.Values.Single();
    }

    public async Task<ExchangeRate> GetExchangeRateAsync(
        CurrencyInfo baseCurrency,
        CurrencyInfo counterCurrency,
        DateTimeOffset asOn,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(baseCurrency);
        ArgumentNullException.ThrowIfNull(counterCurrency);

        var rates = await this.GetRatesAsync(baseCurrency, counterCurrency, asOn, cancellationToken).ConfigureAwait(false);

        return new ExchangeRate(new CurrencyPair(baseCurrency, counterCurrency), asOn, rates.Single().Value);
    }

    public async Task<IEnumerable<ExchangeRate>> GetExchangeRatesAsync(
        DateTimeOffset asOn,
        CancellationToken cancellationToken)
    {
        var rates = await this.GetRatesAsync(baseCurrency: null, counterCurrency: null, asOn, cancellationToken).ConfigureAwait(false);

        return rates.Select(item =>
                new ExchangeRate(new CurrencyPair(item.Key.BaseCurrency, item.Key.CounterCurrency), asOn,
                    item.Value))
            .ToArray();
    }

    private static bool IsSupportedCurrency(string currencyCode) => currencyCode.ToUpperInvariant() switch
    {
        "BTC" or "GGP" or "IMP" or "JEP" => false,
        _ => true,
    };

    private async Task<IDictionary<CurrencyPair, decimal>> GetRatesAsync(
        CurrencyInfo baseCurrency,
        CurrencyInfo counterCurrency,
        DateTimeOffset asOn,
        CancellationToken cancellationToken)
    {
        string requestUrl;

        var difference = this.timeProvider.GetUtcNow() - asOn;

        if (difference < TimeSpan.FromDays(1.0))
        {
            requestUrl = LiveBaseURL;
        }
        else
        {
            requestUrl = HistoricalBaseURL;
            requestUrl += string.Format(CultureInfo.InvariantCulture, "date={0}&", asOn.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture));
        }

        requestUrl += string.Format(CultureInfo.InvariantCulture, "access_key={0}", this.accessKey);

        if (baseCurrency != null)
        {
            requestUrl += string.Format(CultureInfo.InvariantCulture, "&source={0}", baseCurrency.ISOCurrencySymbol);
        }

        if (counterCurrency != null)
        {
            requestUrl += string.Format(CultureInfo.InvariantCulture, "&currencies={0}", counterCurrency.ISOCurrencySymbol);
        }

        var response = await this.httpClient.GetAsync(new Uri(requestUrl), cancellationToken).ConfigureAwait(false);

        _ = response.EnsureSuccessStatusCode();

        var responseJsonString = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);

        var jsonSerializerSettings = new JsonSerializerSettings { Culture = CultureInfo.InvariantCulture };

        var responseJsonObject =
            JsonConvert.DeserializeObject<Dictionary<string, object>>(responseJsonString,
                jsonSerializerSettings);

        var success = (bool)responseJsonObject["success"];

        if (success)
        {
            var result = new Dictionary<CurrencyPair, decimal>();

            var quotes =
                JsonConvert.DeserializeObject<Dictionary<string, decimal>>(
                    responseJsonObject["quotes"].ToString(), jsonSerializerSettings);

            foreach (var quote in quotes)
            {
                var quoteBaseCurrencyCode = quote.Key[..3];
                var quoteCounterCurrencyCode = quote.Key[3..];

                if (IsSupportedCurrency(quoteBaseCurrencyCode) &&
                    IsSupportedCurrency(quoteCounterCurrencyCode) &&
                    !quoteBaseCurrencyCode.Equals(quoteCounterCurrencyCode, StringComparison.OrdinalIgnoreCase))
                {
                    var quoteBaseCurrency = this.currencyFactory.Create(quoteBaseCurrencyCode);
                    var quoteCounterCurrency = this.currencyFactory.Create(quoteCounterCurrencyCode);

                    var quotePair = new CurrencyPair(quoteBaseCurrency, quoteCounterCurrency);

                    result.Add(quotePair, quote.Value);
                }
            }

            return result;
        }

        var errorDictionary =
            JsonConvert.DeserializeObject<Dictionary<string, object>>(responseJsonObject["error"].ToString());

        throw new NotSupportedException(errorDictionary["info"].ToString());
    }
}
