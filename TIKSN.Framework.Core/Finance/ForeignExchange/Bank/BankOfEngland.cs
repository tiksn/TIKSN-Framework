using System.Globalization;
using System.Text;
using System.Xml.Linq;

namespace TIKSN.Finance.ForeignExchange.Bank;

public class BankOfEngland : IBankOfEngland
{
    private static readonly (Dictionary<CurrencyPair, string>, Dictionary<string, CurrencyPair>) SeriesCodesMaps = CreateSeriesCodesMaps();

    private static readonly CompositeFormat UrlFormat =
            CompositeFormat.Parse("https://www.bankofengland.co.uk/boeapps/iadb/fromshowcolumns.asp?CodeVer=new&xml.x=yes&Datefrom={0}&Dateto={1}&SeriesCodes={2}");

    private readonly HttpClient httpClient;
    private readonly TimeProvider timeProvider;

    public BankOfEngland(
        HttpClient httpClient,
        TimeProvider timeProvider)
    {
        this.httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        this.timeProvider = timeProvider ?? throw new ArgumentNullException(nameof(timeProvider));
    }

    private static Dictionary<string, CurrencyPair> Pairs => SeriesCodesMaps.Item2;
    private static Dictionary<CurrencyPair, string> SeriesCodes => SeriesCodesMaps.Item1;

    public async Task<Money> ConvertCurrencyAsync(
        Money baseMoney,
        CurrencyInfo counterCurrency,
        DateTimeOffset asOn,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(baseMoney);
        ArgumentNullException.ThrowIfNull(counterCurrency);

        var exchangeRate = await this.GetExchangeRateAsync(baseMoney.Currency, counterCurrency, asOn, cancellationToken).ConfigureAwait(false);
        var rate = exchangeRate.Rate;

        return new Money(counterCurrency, baseMoney.Amount * rate);
    }

    public async Task<IReadOnlyCollection<CurrencyPair>> GetCurrencyPairsAsync(
        DateTimeOffset asOn,
        CancellationToken cancellationToken)
    {
        var pairs = new List<CurrencyPair>();

        foreach (var pair in SeriesCodes.Keys)
        {
            var rate = await this.GetExchangeRateAsync(
                pair,
                asOn,
                cancellationToken).ConfigureAwait(false);

            if (rate != decimal.Zero)
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
        var pair = new CurrencyPair(baseCurrency, counterCurrency);

        try
        {
            seriesCode = SeriesCodes[pair];
        }
        catch (KeyNotFoundException)
        {
            throw new ArgumentException($"Currency pair {pair} not supported.", nameof(baseCurrency));
        }

        var exchangeRates = await this.GetSeriesCodeExchangeRateAsync(seriesCode, asOn, cancellationToken).ConfigureAwait(false);

        return exchangeRates
            .Where(x => x.Pair == pair)
            .MinByWithTies(x => x.AsOn - asOn)[0];
    }

    public async Task<IEnumerable<ExchangeRate>> GetExchangeRatesAsync(DateTimeOffset asOn, CancellationToken cancellationToken)
    {
        List<ExchangeRate> rates = [];
        foreach (var seriesCode in SeriesCodes)
        {
            var exchangeRates = await this.GetSeriesCodeExchangeRateAsync(seriesCode.Value, asOn, cancellationToken).ConfigureAwait(false);
            rates.AddRange(exchangeRates);
        }

        return rates;
    }

    private static void AddSeriesCode(
        List<(CurrencyPair pair, string serieCode)> codes,
        string baseCountryCode,
        string counterCountryCode,
        string serieCode)
    {
        var baseCountry = new RegionInfo(baseCountryCode);
        var counterCountry = new RegionInfo(counterCountryCode);

        var baseCurrency = new CurrencyInfo(baseCountry);
        var counterCurrency = new CurrencyInfo(counterCountry);

        var pair = new CurrencyPair(baseCurrency, counterCurrency);

        codes.Add((pair, serieCode));
    }

#pragma warning disable MA0051 // Method is too long

    private static (Dictionary<CurrencyPair, string>, Dictionary<string, CurrencyPair>) CreateSeriesCodesMaps()
#pragma warning restore MA0051 // Method is too long
    {
        List<(CurrencyPair pair, string serieCode)> codes = [];

        AddSeriesCode(codes, "en-AU", "en-US", "XUDLADD");
        AddSeriesCode(codes, "en-AU", "en-GB", "XUDLADS");

        AddSeriesCode(codes, "en-CA", "en-GB", "XUDLCDS");

        AddSeriesCode(codes, "zh-CN", "en-US", "XUDLBK73");

        AddSeriesCode(codes, "cs-CZ", "en-US", "XUDLBK27");
        AddSeriesCode(codes, "cs-CZ", "en-GB", "XUDLBK25");

        AddSeriesCode(codes, "da-DK", "en-US", "XUDLDKD");
        AddSeriesCode(codes, "da-DK", "en-GB", "XUDLDKS");

        AddSeriesCode(codes, "de-DE", "en-US", "XUDLERD");
        AddSeriesCode(codes, "de-DE", "en-GB", "XUDLERS");

        AddSeriesCode(codes, "zh-HK", "en-US", "XUDLHDD");
        AddSeriesCode(codes, "zh-HK", "en-GB", "XUDLHDS");

        AddSeriesCode(codes, "hu-HU", "en-US", "XUDLBK35");
        AddSeriesCode(codes, "hu-HU", "en-GB", "XUDLBK33");

        AddSeriesCode(codes, "hi-IN", "en-GB", "XUDLBK97");
        AddSeriesCode(codes, "hi-IN", "en-US", "XUDLBK64");

        AddSeriesCode(codes, "he-IL", "en-GB", "XUDLBK78");
        AddSeriesCode(codes, "he-IL", "en-US", "XUDLBK65");

        AddSeriesCode(codes, "ja-JP", "en-US", "XUDLJYD");
        AddSeriesCode(codes, "ja-JP", "en-GB", "XUDLJYS");

        AddSeriesCode(codes, "ms-MY", "en-GB", "XUDLBK83");
        AddSeriesCode(codes, "ms-MY", "en-US", "XUDLBK66");

        AddSeriesCode(codes, "en-NZ", "en-US", "XUDLNDD");
        AddSeriesCode(codes, "en-NZ", "en-GB", "XUDLNDS");

        AddSeriesCode(codes, "nn-NO", "en-US", "XUDLNKD");
        AddSeriesCode(codes, "nn-NO", "en-GB", "XUDLNKS");

        AddSeriesCode(codes, "pl-PL", "en-US", "XUDLBK49");
        AddSeriesCode(codes, "pl-PL", "en-GB", "XUDLBK47");

        AddSeriesCode(codes, "ar-SA", "en-US", "XUDLSRD");
        AddSeriesCode(codes, "ar-SA", "en-GB", "XUDLSRS");

        AddSeriesCode(codes, "zh-SG", "en-US", "XUDLSGD");
        AddSeriesCode(codes, "zh-SG", "en-GB", "XUDLSGS");

        AddSeriesCode(codes, "af-ZA", "en-US", "XUDLZRD");
        AddSeriesCode(codes, "af-ZA", "en-GB", "XUDLZRS");

        AddSeriesCode(codes, "ko-KR", "en-GB", "XUDLBK93");
        AddSeriesCode(codes, "ko-KR", "en-US", "XUDLBK74");

        AddSeriesCode(codes, "en-GB", "en-US", "XUDLGBD");

        AddSeriesCode(codes, "se-SE", "en-US", "XUDLSKD");
        AddSeriesCode(codes, "se-SE", "en-GB", "XUDLSKS");

        AddSeriesCode(codes, "de-CH", "en-US", "XUDLSFD");
        AddSeriesCode(codes, "de-CH", "en-GB", "XUDLSFS");

        AddSeriesCode(codes, "zh-TW", "en-US", "XUDLTWD");
        AddSeriesCode(codes, "zh-TW", "en-GB", "XUDLTWS");

        AddSeriesCode(codes, "th-TH", "en-GB", "XUDLBK87");
        AddSeriesCode(codes, "th-TH", "en-US", "XUDLBK72");

        AddSeriesCode(codes, "tr-TR", "en-GB", "XUDLBK95");
        AddSeriesCode(codes, "tr-TR", "en-US", "XUDLBK75");

        AddSeriesCode(codes, "en-US", "en-GB", "XUDLUSS");

        AddSeriesCode(codes, "zh-CN", "en-GB", "XUDLBK89");

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
        var pair = Pairs[seriesCode];

        if (asOn > this.timeProvider.GetUtcNow())
        {
            throw new ArgumentException("Exchange rate forecasting are not supported.", nameof(asOn));
        }

        var requestUrl = new Uri(string.Format(CultureInfo.InvariantCulture, UrlFormat,
            ToInternalDataFormat(asOn.AddMonths(-1)),
            ToInternalDataFormat(asOn), seriesCode));

        var responseStream = await this.httpClient.GetStreamAsync(requestUrl, cancellationToken).ConfigureAwait(false);

        var xdoc = XDocument.Load(responseStream);

        List<ExchangeRate> rates = [];

        foreach (var item in xdoc.Element("{http://www.gesmes.org/xml/2002-08-01}Envelope")
            .Element("{https://www.bankofengland.co.uk/website/agg_series}Cube")
            .Elements("{https://www.bankofengland.co.uk/website/agg_series}Cube"))
        {
            var time = item.Attribute("TIME");
            if (time is not null)
            {
                var estimatedRate = item.Attribute("OBS_VALUE");

                if (estimatedRate != null)
                {
                    var year = int.Parse(time.Value.AsSpan(0, 4), CultureInfo.InvariantCulture);
                    var month = int.Parse(time.Value.AsSpan(5, 2), CultureInfo.InvariantCulture);
                    var day = int.Parse(time.Value.AsSpan(8), CultureInfo.InvariantCulture);

                    var itemTime = new DateTimeOffset(year, month, day, 0, 0, 0, TimeSpan.Zero);

                    var reverseRate = decimal.Parse(estimatedRate.Value, CultureInfo.InvariantCulture);
                    var rate = decimal.One / reverseRate;

                    rates.Add(new ExchangeRate(pair, itemTime, rate));
                    rates.Add(new ExchangeRate(pair.Reverse(), itemTime, reverseRate));
                }
            }
        }

        return rates;
    }
}
