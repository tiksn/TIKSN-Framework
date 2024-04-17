using System.Globalization;
using System.Text;
using System.Xml.Linq;
using TIKSN.Globalization;

namespace TIKSN.Finance.ForeignExchange.Bank;

/// <summary>
///     Exchange rate converter of National Bank of Ukraine
/// </summary>
/// <seealso cref="ICurrencyConverter" />
public class NationalBankOfUkraine : INationalBankOfUkraine
{
    private static readonly string[] IgnoreList = ["___"];

    private static readonly RegionInfo Ukraine = new("uk-UA");

    private static readonly CultureInfo UkrainianCulture = new("uk-UA");

    private static readonly CurrencyInfo UkrainianHryvnia = new(Ukraine);

    private static readonly CompositeFormat WebServiceUrlFormat =
                        CompositeFormat.Parse("https://bank.gov.ua/NBUStatService/v1/statdirectory/exchange?date={0:yyyyMMdd}");

    private readonly ICurrencyFactory currencyFactory;
    private readonly HttpClient httpClient;

    public NationalBankOfUkraine(
        HttpClient httpClient,
        ICurrencyFactory currencyFactory)
    {
        this.httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        this.currencyFactory = currencyFactory ?? throw new ArgumentNullException(nameof(currencyFactory));
    }

    /// <summary>
    ///     Converts the currency asynchronous.
    /// </summary>
    /// <param name="baseMoney">The base money.</param>
    /// <param name="counterCurrency">The counter currency.</param>
    /// <param name="asOn">As on.</param>
    /// <param name="cancellationToken">Cancellation Token</param>
    /// <returns>Converted Amount</returns>
    /// <exception cref="ArgumentNullException"></exception>
    public async Task<Money> ConvertCurrencyAsync(
        Money baseMoney,
        CurrencyInfo counterCurrency,
        DateTimeOffset asOn,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(baseMoney);
        ArgumentNullException.ThrowIfNull(counterCurrency);

        var rate = await this.GetExchangeRateAsync(baseMoney.Currency, counterCurrency, asOn, cancellationToken).ConfigureAwait(false);

        return new Money(counterCurrency, baseMoney.Amount * rate);
    }

    /// <summary>
    ///     Gets the currency pairs asynchronous.
    /// </summary>
    /// <param name="asOn">As on.</param>
    /// <param name="cancellationToken">Cancellation Token</param>
    /// <returns>Supported Currency Pairs</returns>
    public async Task<IReadOnlyCollection<CurrencyPair>> GetCurrencyPairsAsync(
        DateTimeOffset asOn,
        CancellationToken cancellationToken)
    {
        var records = await this.GetExchangeRatesAsync(asOn, cancellationToken).ConfigureAwait(false);

        return records.Select(item => item.Pair).ToArray();
    }

    /// <summary>
    ///     Gets the exchange rate asynchronous.
    /// </summary>
    /// <param name="pair">The pair.</param>
    /// <param name="asOn">As on.</param>
    /// <param name="cancellationToken">Cancellation Token</param>
    /// <returns>Exchange Rate</returns>
    /// <exception cref="ArgumentNullException"></exception>
    public Task<decimal> GetExchangeRateAsync(
        CurrencyPair pair,
        DateTimeOffset asOn,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(pair);

        return this.GetExchangeRateAsync(pair.BaseCurrency, pair.CounterCurrency, asOn, cancellationToken);
    }

    public async Task<IReadOnlyCollection<ExchangeRate>> GetExchangeRatesAsync(
        DateTimeOffset asOn,
        CancellationToken cancellationToken)
    {
        var requestUri = new Uri(string.Format(UkrainianCulture, WebServiceUrlFormat, asOn));
        var response = await this.httpClient.GetStringAsync(requestUri, cancellationToken).ConfigureAwait(false);
        var xdocument = XDocument.Parse(response);
        var result = new HashSet<ExchangeRate>();

        foreach (var currencyElement in xdocument.Element("exchange").Elements("currency"))
        {
            var currencyCode = currencyElement.Element("cc").Value;

            if (IgnoreList.Contains(currencyCode, StringComparer.OrdinalIgnoreCase))
            {
                continue;
            }

            var rate = decimal.Parse(currencyElement.Element("rate").Value, CultureInfo.InvariantCulture);

            if (!string.IsNullOrEmpty(currencyCode))
            {
                _ = result.Add(new ExchangeRate(
                    new CurrencyPair(this.currencyFactory.Create(currencyCode), UkrainianHryvnia), asOn,
                    rate));
                _ = result.Add(new ExchangeRate(
                    new CurrencyPair(UkrainianHryvnia, this.currencyFactory.Create(currencyCode)), asOn,
                    decimal.One / rate));
            }
        }

        return result;
    }

    private static NotSupportedException CreatePairNotSupportedException(
        CurrencyInfo baseCurrency,
        CurrencyInfo counterCurrency)
    {
        var ex = new NotSupportedException(
            "Currency pair is not supported by National Bank of Ukraine currency converter for given date.");

        ex.Data.Add("Base Currency", baseCurrency?.ISOCurrencySymbol);
        ex.Data.Add("Counter Currency", counterCurrency?.ISOCurrencySymbol);

        return ex;
    }

    private async Task<decimal> GetExchangeRateAsync(
        CurrencyInfo baseCurrency,
        CurrencyInfo counterCurrency,
        DateTimeOffset asOn, CancellationToken cancellationToken)
    {
        if (counterCurrency == UkrainianHryvnia)
        {
            return await this.GetExchangeRateAsync(baseCurrency, asOn, cancellationToken).ConfigureAwait(false);
        }

        if (baseCurrency == UkrainianHryvnia)
        {
            var counterRate = await this.GetExchangeRateAsync(counterCurrency, asOn, cancellationToken).ConfigureAwait(false);

            return 1m / counterRate;
        }

        throw CreatePairNotSupportedException(baseCurrency, counterCurrency);
    }

    private async Task<decimal> GetExchangeRateAsync(
        CurrencyInfo currency,
        DateTimeOffset asOn,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(currency);

        var records = await this.GetExchangeRatesAsync(asOn, cancellationToken).ConfigureAwait(false);
        var record = records
            .SingleOrDefault(item => item.Pair.BaseCurrency == currency)
            ?? throw CreatePairNotSupportedException(baseCurrency: null, currency);

        return record.Rate;
    }
}
