using System.Diagnostics;
using System.Globalization;
using System.Xml.Linq;
using TIKSN.Globalization;

namespace TIKSN.Finance.ForeignExchange.Bank;

public class SwissNationalBank : ISwissNationalBank
{
    private const string RSSURL = "https://www.snb.ch/selector/en/mmr/exfeed/rss";
    private static readonly Dictionary<(Uri RssUrl, DateOnly RequestDate), XDocument> ExchangeRateDocumentCache = [];
    private static readonly SemaphoreSlim ExchangeRateDocumentSemaphore = new(initialCount: 1, maxCount: 1);
    private static readonly CurrencyInfo SwissFranc = new(new RegionInfo("de-CH"));
    private static readonly Uri RssUrl = new(RSSURL);


    private readonly ICurrencyFactory currencyFactory;
    private readonly ICurrencyPairFactory currencyPairFactory;
    private readonly Dictionary<CurrencyInfo, Tuple<DateTimeOffset, decimal>> foreignRates;
    private readonly HttpClient httpClient;
    private readonly TimeProvider timeProvider;

    public SwissNationalBank(
        HttpClient httpClient,
        ICurrencyFactory currencyFactory,
        ICurrencyPairFactory currencyPairFactory,
        TimeProvider timeProvider)
    {
        this.foreignRates = [];
        this.httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        this.currencyFactory = currencyFactory ?? throw new ArgumentNullException(nameof(currencyFactory));
        this.currencyPairFactory = currencyPairFactory ?? throw new ArgumentNullException(nameof(currencyPairFactory));
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

        await this.FetchOnDemandAsync(cancellationToken).ConfigureAwait(false);

        var rate = this.GetRate(baseMoney.Currency, counterCurrency, asOn);

        return new Money(counterCurrency, baseMoney.Amount * rate);
    }

    public async Task<IReadOnlyCollection<CurrencyPair>> GetCurrencyPairsAsync(
        DateTimeOffset asOn,
        CancellationToken cancellationToken)
    {
        await this.FetchOnDemandAsync(cancellationToken).ConfigureAwait(false);

        var pairs = new List<CurrencyPair>();

        foreach (var currency in this.FilterByDate(asOn).Select(x => x.Key))
        {
            pairs.Add(this.currencyPairFactory.Create(SwissFranc, currency));
            pairs.Add(this.currencyPairFactory.Create(currency, SwissFranc));
        }

        return pairs;
    }

    public async Task<decimal> GetExchangeRateAsync(
        CurrencyPair pair,
        DateTimeOffset asOn,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(pair);

        await this.FetchOnDemandAsync(cancellationToken).ConfigureAwait(false);

        return this.GetRate(pair.BaseCurrency, pair.CounterCurrency, asOn);
    }

    public async Task<IReadOnlyCollection<ExchangeRate>> GetExchangeRatesAsync(
        DateTimeOffset asOn,
        CancellationToken cancellationToken)
    {
        var result = new List<ExchangeRate>();

        var xdoc = await this.GetExchangeRateDocumentAsync(cancellationToken).ConfigureAwait(false);

        lock (this.foreignRates)
        {
            foreach (var itemElement in xdoc?.Element("rss")?.Element("channel")?.Elements("item") ?? [])
            {
                var dateElement = itemElement.Element("{http://purl.org/dc/elements/1.1/}date");
                var exchangeRateElement = itemElement
                    ?.Element("{http://www.cbwiki.net/wiki/index.php/Specification_1.2/}statistics")
                    ?.Element("{http://www.cbwiki.net/wiki/index.php/Specification_1.2/}exchangeRate");
                var valueElement = exchangeRateElement
                    ?.Element("{http://www.cbwiki.net/wiki/index.php/Specification_1.2/}observation")
                    ?.Element("{http://www.cbwiki.net/wiki/index.php/Specification_1.2/}value");
                var targetCurrencyElement =
                    exchangeRateElement?.Element(
                        "{http://www.cbwiki.net/wiki/index.php/Specification_1.2/}targetCurrency");

                var date = DateTimeOffset.Parse(dateElement?.Value ?? string.Empty, CultureInfo.InvariantCulture);
                var currencyCode = targetCurrencyElement?.Value ?? string.Empty;
                var rate = decimal.Parse(valueElement?.Value ?? string.Empty, CultureInfo.InvariantCulture);

                var currency = this.currencyFactory.Create(currencyCode);

                Debug.Assert(string.Equals(exchangeRateElement
                        ?.Element("{http://www.cbwiki.net/wiki/index.php/Specification_1.2/}observation")
                        ?.Element("{http://www.cbwiki.net/wiki/index.php/Specification_1.2/}unit")?.Value,
                    "CHF",
                    StringComparison.Ordinal));

                this.foreignRates[currency] = new Tuple<DateTimeOffset, decimal>(date, rate);
                result.Add(new ExchangeRate(this.currencyPairFactory.Create(currency, SwissFranc), date, rate));
            }
        }

        return result;
    }

    private async Task FetchOnDemandAsync(CancellationToken cancellationToken)
    {
        if (this.foreignRates.Count == 0 ||
            this.foreignRates.Any(r => r.Value.Item1.Date == this.timeProvider.GetUtcNow().Date))
        {
            _ = await this.GetExchangeRatesAsync(this.timeProvider.GetUtcNow(), cancellationToken)
                .ConfigureAwait(false);
        }
    }

    private Dictionary<CurrencyInfo, decimal> FilterByDate(DateTimeOffset asOn)
    {
        if (asOn > this.timeProvider.GetUtcNow())
        {
            throw new ArgumentException("Exchange rate forecasting not supported.", nameof(asOn));
        }

        var maxDate = this.foreignRates.Max(r => r.Value.Item1);
        var minDate = this.foreignRates.Min(r => r.Value.Item1);

        if (asOn < minDate)
        {
            throw new ArgumentException("Exchange rate history not supported.", nameof(asOn));
        }

        DateTimeOffset filterDate;

        if (asOn > maxDate)
        {
            filterDate = maxDate;
        }
        else
        {
            filterDate = asOn;
        }

        var filteredResults = this.foreignRates.Where(r => r.Value.Item1.Date == filterDate.Date)
            .Select(r => new Tuple<CurrencyInfo, decimal>(r.Key, r.Value.Item2));

        var results = new Dictionary<CurrencyInfo, decimal>();

        foreach (var filteredResult in filteredResults)
        {
            results.Add(filteredResult.Item1, filteredResult.Item2);
        }

        return results;
    }

    private async Task<XDocument> GetExchangeRateDocumentAsync(CancellationToken cancellationToken)
    {
        var cacheKey = (RssUrl, DateOnly.FromDateTime(this.timeProvider.GetUtcNow().Date));

        await ExchangeRateDocumentSemaphore.WaitAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            if (ExchangeRateDocumentCache.TryGetValue(cacheKey, out var cachedDocument))
            {
                return cachedDocument;
            }

            var document = await this.GetExchangeRateDocumentCoreAsync(cancellationToken).ConfigureAwait(false);
            ExchangeRateDocumentCache[cacheKey] = document;

            return document;
        }
        finally
        {
            _ = ExchangeRateDocumentSemaphore.Release();
        }
    }

    private async Task<XDocument> GetExchangeRateDocumentCoreAsync(CancellationToken cancellationToken)
    {
        var responseStream =
            await this.httpClient.GetStreamAsync(RssUrl, cancellationToken).ConfigureAwait(false);

        await using (responseStream.ConfigureAwait(false))
        {
            return XDocument.Load(responseStream);
        }
    }

    private decimal GetRate(
        CurrencyInfo baseCurrency,
        CurrencyInfo counterCurrency,
        DateTimeOffset asOn)
    {
        var filtered = this.FilterByDate(asOn);

        if (baseCurrency == SwissFranc)
        {
            if (filtered.TryGetValue(counterCurrency, out var counterRate))
            {
                return decimal.One / counterRate;
            }
        }
        else if (counterCurrency == SwissFranc &&
                 filtered.TryGetValue(baseCurrency, out var rate))
        {
            return rate;
        }

        throw new ArgumentException(
            "Currency pair not supported.",
            nameof(baseCurrency),
            new ArgumentException(
                "Currency pair not supported.",
                nameof(counterCurrency)));
    }
}
