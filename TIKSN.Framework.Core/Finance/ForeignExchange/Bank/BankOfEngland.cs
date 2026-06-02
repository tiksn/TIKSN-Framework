using System.Globalization;
using System.Text;
using System.Xml.Linq;

namespace TIKSN.Finance.ForeignExchange.Bank;

public class BankOfEngland : IBankOfEngland
{
    private static readonly Dictionary<Uri, IReadOnlyList<ExchangeRate>> ExchangeRatesCache = [];
    private static readonly SemaphoreSlim ExchangeRatesSemaphore = new(initialCount: 1, maxCount: 1);

    private static readonly CompositeFormat UrlFormat =
        CompositeFormat.Parse(
            "https://www.bankofengland.co.uk/boeapps/database/_iadb-fromshowcolumns.asp?CodeVer=new&xml.x=yes&Datefrom={0}&Dateto={1}&SeriesCodes={2}&VPD=Y&VFD=N");

    private readonly ICurrencyPairFactory currencyPairFactory;
    private readonly HttpClient httpClient;
    private readonly (Dictionary<CurrencyPair, string>, Dictionary<string, CurrencyPair>) seriesCodesMaps;
    private readonly TimeProvider timeProvider;

    public BankOfEngland(
        HttpClient httpClient,
        ICurrencyPairFactory currencyPairFactory,
        TimeProvider timeProvider)
    {
        this.httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        this.currencyPairFactory = currencyPairFactory ?? throw new ArgumentNullException(nameof(currencyPairFactory));
        this.seriesCodesMaps = CreateSeriesCodesMaps(this.currencyPairFactory);
        this.timeProvider = timeProvider ?? throw new ArgumentNullException(nameof(timeProvider));
    }

    private Dictionary<string, CurrencyPair> Pairs => this.seriesCodesMaps.Item2;
    private Dictionary<CurrencyPair, string> SeriesCodes => this.seriesCodesMaps.Item1;

    public async Task<Money> ConvertCurrencyAsync(
        Money baseMoney,
        CurrencyInfo counterCurrency,
        DateTimeOffset asOn,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(baseMoney);
        ArgumentNullException.ThrowIfNull(counterCurrency);

        var exchangeRate = await this.GetExchangeRateAsync(baseMoney.Currency, counterCurrency, asOn, cancellationToken)
            .ConfigureAwait(false);
        var rate = exchangeRate.Rate;

        return new Money(counterCurrency, baseMoney.Amount * rate);
    }

    public async Task<IReadOnlyCollection<CurrencyPair>> GetCurrencyPairsAsync(
        DateTimeOffset asOn,
        CancellationToken cancellationToken)
    {
        var pairs = new List<CurrencyPair>();

        foreach (var pair in this.SeriesCodes.Keys)
        {
            var rates = await this.GetSeriesCodeExchangeRateAsync(
                this.SeriesCodes[pair],
                asOn,
                cancellationToken).ConfigureAwait(false);

            if (rates.Count != 0)
            {
                pairs.Add(pair);
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

        var exchangeRate = await this.GetExchangeRateAsync(
            pair.BaseCurrency,
            pair.CounterCurrency,
            asOn,
            cancellationToken).ConfigureAwait(false);
        return exchangeRate.Rate;
    }

    public async Task<ExchangeRate> GetExchangeRateAsync(
        CurrencyInfo baseCurrency,
        CurrencyInfo counterCurrency,
        DateTimeOffset asOn,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(baseCurrency);
        ArgumentNullException.ThrowIfNull(counterCurrency);

        if (asOn > this.timeProvider.GetUtcNow())
        {
            throw new ArgumentException("Exchange rate forecasting are not supported.", nameof(asOn));
        }

        string seriesCode;
        var pair = this.currencyPairFactory.Create(baseCurrency, counterCurrency);

        try
        {
            seriesCode = this.SeriesCodes[pair];
        }
        catch (KeyNotFoundException)
        {
            throw new ArgumentException($"Currency pair {pair} not supported.", nameof(baseCurrency));
        }

        var exchangeRates = await this.GetSeriesCodeExchangeRateAsync(seriesCode, asOn, cancellationToken)
            .ConfigureAwait(false);

        var exchangeRate = exchangeRates
            .Where(x => x.Pair == pair && x.AsOn <= asOn)
            .OrderByDescending(x => x.AsOn)
            .FirstOrDefault();

        return exchangeRate ?? throw new InvalidOperationException(
            string.Create(CultureInfo.InvariantCulture,
                $"No Bank of England exchange rate found for {pair} on or before {asOn:O}."));
    }

    public async Task<IReadOnlyCollection<ExchangeRate>> GetExchangeRatesAsync(DateTimeOffset asOn,
        CancellationToken cancellationToken)
    {
        List<ExchangeRate> rates = [];
        foreach (var seriesCode in this.SeriesCodes)
        {
            var exchangeRates = await this.GetSeriesCodeExchangeRateAsync(seriesCode.Value, asOn, cancellationToken)
                .ConfigureAwait(false);
            rates.AddRange(exchangeRates);
        }

        return rates;
    }

    private static void AddSeriesCode(
        List<(CurrencyPair pair, string serieCode)> codes,
        ICurrencyPairFactory currencyPairFactory,
        string baseCountryCode,
        string counterCountryCode,
        string serieCode)
    {
        var baseCountry = new RegionInfo(baseCountryCode);
        var counterCountry = new RegionInfo(counterCountryCode);

        var baseCurrency = new CurrencyInfo(baseCountry);
        var counterCurrency = new CurrencyInfo(counterCountry);

        var pair = currencyPairFactory.Create(baseCurrency, counterCurrency);

        codes.Add((pair, serieCode));
    }

#pragma warning disable MA0051 // Method is too long

    private static (Dictionary<CurrencyPair, string>, Dictionary<string, CurrencyPair>) CreateSeriesCodesMaps(
        ICurrencyPairFactory currencyPairFactory)
#pragma warning restore MA0051 // Method is too long
    {
        List<(CurrencyPair pair, string serieCode)> codes = [];

        AddSeriesCode(codes, currencyPairFactory, "en-AU", "en-US", "XUDLADD");
        AddSeriesCode(codes, currencyPairFactory, "en-AU", "en-GB", "XUDLADS");

        AddSeriesCode(codes, currencyPairFactory, "en-CA", "en-GB", "XUDLCDS");

        AddSeriesCode(codes, currencyPairFactory, "zh-CN", "en-US", "XUDLBK73");

        AddSeriesCode(codes, currencyPairFactory, "cs-CZ", "en-US", "XUDLBK27");
        AddSeriesCode(codes, currencyPairFactory, "cs-CZ", "en-GB", "XUDLBK25");

        AddSeriesCode(codes, currencyPairFactory, "da-DK", "en-US", "XUDLDKD");
        AddSeriesCode(codes, currencyPairFactory, "da-DK", "en-GB", "XUDLDKS");

        AddSeriesCode(codes, currencyPairFactory, "de-DE", "en-US", "XUDLERD");
        AddSeriesCode(codes, currencyPairFactory, "de-DE", "en-GB", "XUDLERS");

        AddSeriesCode(codes, currencyPairFactory, "zh-HK", "en-US", "XUDLHDD");
        AddSeriesCode(codes, currencyPairFactory, "zh-HK", "en-GB", "XUDLHDS");

        AddSeriesCode(codes, currencyPairFactory, "hu-HU", "en-US", "XUDLBK35");
        AddSeriesCode(codes, currencyPairFactory, "hu-HU", "en-GB", "XUDLBK33");

        AddSeriesCode(codes, currencyPairFactory, "hi-IN", "en-GB", "XUDLBK97");
        AddSeriesCode(codes, currencyPairFactory, "hi-IN", "en-US", "XUDLBK64");

        AddSeriesCode(codes, currencyPairFactory, "he-IL", "en-GB", "XUDLBK78");
        AddSeriesCode(codes, currencyPairFactory, "he-IL", "en-US", "XUDLBK65");

        AddSeriesCode(codes, currencyPairFactory, "ja-JP", "en-US", "XUDLJYD");
        AddSeriesCode(codes, currencyPairFactory, "ja-JP", "en-GB", "XUDLJYS");

        AddSeriesCode(codes, currencyPairFactory, "ms-MY", "en-GB", "XUDLBK83");
        AddSeriesCode(codes, currencyPairFactory, "ms-MY", "en-US", "XUDLBK66");

        AddSeriesCode(codes, currencyPairFactory, "en-NZ", "en-US", "XUDLNDD");
        AddSeriesCode(codes, currencyPairFactory, "en-NZ", "en-GB", "XUDLNDS");

        AddSeriesCode(codes, currencyPairFactory, "nn-NO", "en-US", "XUDLNKD");
        AddSeriesCode(codes, currencyPairFactory, "nn-NO", "en-GB", "XUDLNKS");

        AddSeriesCode(codes, currencyPairFactory, "pl-PL", "en-US", "XUDLBK49");
        AddSeriesCode(codes, currencyPairFactory, "pl-PL", "en-GB", "XUDLBK47");

        AddSeriesCode(codes, currencyPairFactory, "ar-SA", "en-US", "XUDLSRD");
        AddSeriesCode(codes, currencyPairFactory, "ar-SA", "en-GB", "XUDLSRS");

        AddSeriesCode(codes, currencyPairFactory, "zh-SG", "en-US", "XUDLSGD");
        AddSeriesCode(codes, currencyPairFactory, "zh-SG", "en-GB", "XUDLSGS");

        AddSeriesCode(codes, currencyPairFactory, "af-ZA", "en-US", "XUDLZRD");
        AddSeriesCode(codes, currencyPairFactory, "af-ZA", "en-GB", "XUDLZRS");

        AddSeriesCode(codes, currencyPairFactory, "ko-KR", "en-GB", "XUDLBK93");
        AddSeriesCode(codes, currencyPairFactory, "ko-KR", "en-US", "XUDLBK74");

        AddSeriesCode(codes, currencyPairFactory, "en-GB", "en-US", "XUDLGBD");

        AddSeriesCode(codes, currencyPairFactory, "se-SE", "en-US", "XUDLSKD");
        AddSeriesCode(codes, currencyPairFactory, "se-SE", "en-GB", "XUDLSKS");

        AddSeriesCode(codes, currencyPairFactory, "de-CH", "en-US", "XUDLSFD");
        AddSeriesCode(codes, currencyPairFactory, "de-CH", "en-GB", "XUDLSFS");

        AddSeriesCode(codes, currencyPairFactory, "zh-TW", "en-US", "XUDLTWD");
        AddSeriesCode(codes, currencyPairFactory, "zh-TW", "en-GB", "XUDLTWS");

        AddSeriesCode(codes, currencyPairFactory, "th-TH", "en-GB", "XUDLBK87");
        AddSeriesCode(codes, currencyPairFactory, "th-TH", "en-US", "XUDLBK72");

        AddSeriesCode(codes, currencyPairFactory, "tr-TR", "en-GB", "XUDLBK95");
        AddSeriesCode(codes, currencyPairFactory, "tr-TR", "en-US", "XUDLBK75");

        AddSeriesCode(codes, currencyPairFactory, "en-US", "en-GB", "XUDLUSS");

        AddSeriesCode(codes, currencyPairFactory, "zh-CN", "en-GB", "XUDLBK89");

        return ValueTuple.Create(
            codes.ToDictionary(k => k.pair, v => v.serieCode),
            codes.ToDictionary(k => k.serieCode, v => v.pair, StringComparer.Ordinal));
    }

    private static string ToInternalDataFormat(DateTimeOffset dt) =>
        dt.ToString("dd/MMM/yyyy", CultureInfo.InvariantCulture);

    private async Task<IReadOnlyList<ExchangeRate>> GetSeriesCodeExchangeRateAsync(
        string seriesCode,
        DateTimeOffset asOn,
        CancellationToken cancellationToken)
    {
        if (asOn > this.timeProvider.GetUtcNow())
        {
            throw new ArgumentException("Exchange rate forecasting are not supported.", nameof(asOn));
        }

        var requestUrl = new Uri(string.Format(CultureInfo.InvariantCulture, UrlFormat,
            ToInternalDataFormat(asOn.AddMonths(-1)),
            ToInternalDataFormat(asOn), seriesCode));

        await ExchangeRatesSemaphore.WaitAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            if (ExchangeRatesCache.TryGetValue(requestUrl, out var cachedRates))
            {
                return cachedRates;
            }

            var rates = await this
                .GetSeriesCodeExchangeRateCoreAsync(seriesCode, requestUrl, cancellationToken)
                .ConfigureAwait(false);
            ExchangeRatesCache[requestUrl] = rates;

            return rates;
        }
        finally
        {
            _ = ExchangeRatesSemaphore.Release();
        }
    }

    private async Task<IReadOnlyList<ExchangeRate>> GetSeriesCodeExchangeRateCoreAsync(
        string seriesCode,
        Uri requestUrl,
        CancellationToken cancellationToken)
    {
        var pair = this.Pairs[seriesCode];

        var responseStream = await this.httpClient.GetStreamAsync(requestUrl, cancellationToken).ConfigureAwait(false);
        XDocument xdoc;
        await using (responseStream.ConfigureAwait(false))
        {
            xdoc = XDocument.Load(responseStream);
        }

        List<ExchangeRate> rates = [];

        foreach (var item in xdoc
                     .Descendants()
                     .Where(x => x.Name.LocalName == "Cube" && x.Attribute("TIME") is not null))
        {
            var time = item.Attribute("TIME");
            if (time is not null)
            {
                var estimatedRate = item.Attribute("OBS_VALUE");

                if (estimatedRate != null)
                {
                    var year = int.Parse(time.Value.AsSpan(start: 0, length: 4), CultureInfo.InvariantCulture);
                    var month = int.Parse(time.Value.AsSpan(start: 5, length: 2), CultureInfo.InvariantCulture);
                    var day = int.Parse(time.Value.AsSpan(8), CultureInfo.InvariantCulture);

                    var itemTime = new DateTimeOffset(year, month, day, hour: 0, minute: 0, second: 0, TimeSpan.Zero);

                    var reverseRate = decimal.Parse(estimatedRate.Value, CultureInfo.InvariantCulture);
                    var rate = decimal.One / reverseRate;

                    rates.Add(new ExchangeRate(pair, itemTime, rate));
                    rates.Add(new ExchangeRate(this.currencyPairFactory.Reverse(pair), itemTime, reverseRate));
                }
            }
        }

        return rates;
    }
}
