using System.Globalization;
using System.Xml.Linq;
using TIKSN.Globalization;

namespace TIKSN.Finance.ForeignExchange.Bank;

public class CentralBankOfArmenia : ICentralBankOfArmenia
{
    private static readonly CurrencyInfo AMD = new(new RegionInfo("hy-AM"));

    private static readonly Uri RSS =
            new("https://www.cba.am/_layouts/rssreader.aspx?rss=280F57B8-763C-4EE4-90E0-8136C13E47DA");

    private readonly ICurrencyFactory currencyFactory;
    private readonly HttpClient httpClient;
    private readonly Dictionary<CurrencyInfo, decimal> oneWayRates;
    private readonly TimeProvider timeProvider;
    private DateTimeOffset lastFetchDate;
    private DateTimeOffset? publicationDate;

    public CentralBankOfArmenia(
        HttpClient httpClient,
        ICurrencyFactory currencyFactory,
        TimeProvider timeProvider)
    {
        this.oneWayRates = [];

        this.lastFetchDate = DateTimeOffset.MinValue;
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

        var pair = new CurrencyPair(baseMoney.Currency, counterCurrency);

        var rate = await this.GetExchangeRateAsync(pair, asOn, cancellationToken).ConfigureAwait(false);

        return new Money(counterCurrency, baseMoney.Amount * rate);
    }

    public async Task<IEnumerable<CurrencyPair>> GetCurrencyPairsAsync(
        DateTimeOffset asOn,
        CancellationToken cancellationToken)
    {
        await this.FetchOnDemandAsync(asOn, cancellationToken).ConfigureAwait(false);

        var rates = new List<CurrencyPair>();

        foreach (var rate in this.oneWayRates)
        {
            rates.Add(new CurrencyPair(rate.Key, AMD));
            rates.Add(new CurrencyPair(AMD, rate.Key));
        }

        return rates;
    }

    public async Task<decimal> GetExchangeRateAsync(
        CurrencyPair pair,
        DateTimeOffset asOn,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(pair);

        await this.FetchOnDemandAsync(asOn, cancellationToken).ConfigureAwait(false);

        if (pair.CounterCurrency == AMD)
        {
            if (this.oneWayRates.TryGetValue(pair.BaseCurrency, out var rate))
            {
                return rate;
            }
        }
        else if (this.oneWayRates.TryGetValue(pair.CounterCurrency, out var counterRate))
        {
            return decimal.One / counterRate;
        }

        throw new ArgumentException("Currency pair was not found.", nameof(pair));
    }

    public async Task<IEnumerable<ExchangeRate>> GetExchangeRatesAsync(
        DateTimeOffset asOn,
        CancellationToken cancellationToken)
    {
        this.ValodateDate(asOn);

        var result = new List<ExchangeRate>();

        var responseStream = await this.httpClient.GetStreamAsync(RSS, cancellationToken).ConfigureAwait(false);

        var xdoc = XDocument.Load(responseStream);

        lock (this.oneWayRates)
        {
            foreach (var item in xdoc.Element("rss").Element("channel").Elements("item"))
            {
                var title = item.Element("title");
                var pubDate = item.Element("pubDate");

                var titleParts = title.Value.Split('-');

                var currencyCode = titleParts[0].Trim().ToUpper(CultureInfo.InvariantCulture);
                var baseUnit = decimal.Parse(titleParts[1], CultureInfo.InvariantCulture);
                var counterUnit = decimal.Parse(titleParts[2], CultureInfo.InvariantCulture);

                if (string.Equals(currencyCode, "BRC", StringComparison.Ordinal))
                {
                    currencyCode = "BRL";
                }

                if (string.Equals(currencyCode, "LVL", StringComparison.Ordinal))
                {
                    continue;
                }

                if (string.Equals(currencyCode, "SDR", StringComparison.OrdinalIgnoreCase))
                {
                    currencyCode = "XDR";
                }

                var publishedAt = DateTimeOffset.Parse(pubDate.Value, CultureInfo.InvariantCulture);

                if (baseUnit != decimal.Zero && counterUnit != decimal.Zero)
                {
                    var rate = counterUnit / baseUnit;

                    var currency = this.currencyFactory.Create(currencyCode);
                    this.oneWayRates[currency] = rate;
                    result.Add(new ExchangeRate(new CurrencyPair(AMD, currency), publishedAt, rate));
                    result.Add(new ExchangeRate(new CurrencyPair(currency, AMD), publishedAt,
                        baseUnit / counterUnit));
                }

                if (!this.publicationDate.HasValue)
                {
                    this.publicationDate = publishedAt;
                }
            }

            this.lastFetchDate = this.timeProvider.GetUtcNow(); // this should stay at the end
        }

        return result;
    }

    private async Task FetchOnDemandAsync(DateTimeOffset asOn, CancellationToken cancellationToken)
    {
        if (!this.publicationDate.HasValue)
        {
            _ = await this.GetExchangeRatesAsync(asOn, cancellationToken).ConfigureAwait(false);
        }
        else if (this.timeProvider.GetUtcNow() - this.lastFetchDate > TimeSpan.FromDays(1d))
        {
            _ = await this.GetExchangeRatesAsync(asOn, cancellationToken).ConfigureAwait(false);
        }
    }

    private void ValodateDate(DateTimeOffset asOn)
    {
        if (asOn > this.timeProvider.GetUtcNow())
        {
            throw new ArgumentException("Exchange rate forecasting are not supported.", nameof(asOn));
        }

        if ((this.publicationDate.HasValue && asOn < this.publicationDate.Value) ||
            asOn < this.timeProvider.GetUtcNow().AddDays(-1))
        {
            throw new ArgumentException("Exchange rate history are not supported.", nameof(asOn));
        }
    }
}
