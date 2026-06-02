using System.Globalization;
using System.Xml.Linq;
using TIKSN.Globalization;

namespace TIKSN.Finance.ForeignExchange.Bank;

public class EuropeanCentralBank : IEuropeanCentralBank
{
    private static readonly TimeZoneInfo EuropeanCentralBankTimeZone = CreateEuropeanCentralBankTimeZone();
    private static readonly Uri DailyRatesUrl = new("https://www.ecb.europa.eu/stats/eurofxref/eurofxref-daily.xml");

    private static readonly CurrencyInfo Euro = new(new RegionInfo("de-DE"));

    private static readonly SemaphoreSlim ExchangeRateDocumentSemaphore = new(initialCount: 1, maxCount: 1);

    private static readonly Dictionary<(Uri RequestURL, DateOnly RequestDate), XDocument> ExchangeRateDocuments = [];


    private static readonly Uri Last90DaysRatesUrl =
        new("https://www.ecb.europa.eu/stats/eurofxref/eurofxref-hist-90d.xml");

    private static readonly Uri Since1999RatesUrl = new("https://www.ecb.europa.eu/stats/eurofxref/eurofxref-hist.xml");
    private readonly ICurrencyFactory currencyFactory;
    private readonly HttpClient httpClient;
    private readonly TimeProvider timeProvider;

    public EuropeanCentralBank(
        HttpClient httpClient,
        ICurrencyFactory currencyFactory,
        TimeProvider timeProvider)
    {
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

    public async Task<IReadOnlyCollection<CurrencyPair>> GetCurrencyPairsAsync(
        DateTimeOffset asOn,
        CancellationToken cancellationToken)
    {
        this.VerifyDate(asOn);

        var rates = await this.GetExchangeRatesAsync(asOn, cancellationToken).ConfigureAwait(false);

        var result = new List<CurrencyPair>();

        foreach (var pair in rates.Select(x => x.Pair))
        {
            result.Add(pair);
            result.Add(new CurrencyPair(pair.CounterCurrency, pair.BaseCurrency));
        }

        return result;
    }

    public async Task<decimal> GetExchangeRateAsync(
        CurrencyPair pair,
        DateTimeOffset asOn,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(pair);

        this.VerifyDate(asOn);

        var rates = await this.GetExchangeRatesAsync(asOn, cancellationToken).ConfigureAwait(false);

        var rate = rates.SingleOrDefault(item => item.Pair == pair);
        if (rate != null)
        {
            return rate.Rate;
        }

        var reverseRate = rates.SingleOrDefault(item => item.Pair.Reverse() == pair);
        if (reverseRate != null)
        {
            return reverseRate.Reverse().Rate;
        }

        throw new ArgumentException($"Currency pair '{pair}' is not found.", nameof(pair));
    }

    public async Task<IReadOnlyCollection<ExchangeRate>> GetExchangeRatesAsync(
        DateTimeOffset asOn,
        CancellationToken cancellationToken)
    {
        var requestURL = GetRatesUrl(asOn, this.timeProvider);

        var xdoc = await this.GetExchangeRateDocumentAsync(requestURL, cancellationToken).ConfigureAwait(false);

        var groupsCubes = xdoc
            ?.Element("{http://www.gesmes.org/xml/2002-08-01}Envelope")
            ?.Element("{http://www.ecb.int/vocabulary/2002-08-01/eurofxref}Cube")
            ?.Elements("{http://www.ecb.int/vocabulary/2002-08-01/eurofxref}Cube") ?? [];

        var asOnDate = GetCentralEuropeanDate(asOn);
        var groupCubes = groupsCubes
            .Select(x =>
                new Tuple<XElement, DateOnly>(x,
                    DateOnly.Parse(x?.Attribute("time")?.Value ?? string.Empty, CultureInfo.InvariantCulture)))
            .Where(z => z.Item2 <= asOnDate)
            .OrderByDescending(y => y.Item2)
            .Select(x => (x.Item1, x.Item2))
            .First();

        var rates = new List<ExchangeRate>();

        foreach (var rateCube in groupCubes.Item1?.Elements(
                     "{http://www.ecb.int/vocabulary/2002-08-01/eurofxref}Cube") ?? [])
        {
            var currencyCode = rateCube?.Attribute("currency")?.Value ?? string.Empty;
            var rate = decimal.Parse(rateCube?.Attribute("rate")?.Value ?? string.Empty, CultureInfo.InvariantCulture);

            rates.Add(new ExchangeRate(new CurrencyPair(Euro, this.currencyFactory.Create(currencyCode)),
                CreateEuropeanCentralBankDateTimeOffset(groupCubes.Item2), rate));
        }

        return rates;
    }

    private static DateTimeOffset CreateEuropeanCentralBankDateTimeOffset(DateOnly date)
    {
        var localDateTime = date.ToDateTime(TimeOnly.MinValue);

        return new DateTimeOffset(localDateTime, EuropeanCentralBankTimeZone.GetUtcOffset(localDateTime));
    }

    private static TimeZoneInfo CreateEuropeanCentralBankTimeZone()
    {
        try
        {
            return TimeZoneInfo.FindSystemTimeZoneById("Europe/Frankfurt");
        }
        catch (TimeZoneNotFoundException)
        {
            return TimeZoneInfo.FindSystemTimeZoneById("W. Europe Standard Time");
        }
        catch (InvalidTimeZoneException)
        {
            return TimeZoneInfo.FindSystemTimeZoneById("W. Europe Standard Time");
        }
    }

    private static DateOnly GetCentralEuropeanDate(DateTimeOffset dateTime) =>
        DateOnly.FromDateTime(TimeZoneInfo.ConvertTime(dateTime, EuropeanCentralBankTimeZone).DateTime);

    private static Uri GetRatesUrl(DateTimeOffset asOn, TimeProvider timeProvider)
    {
        var asOnDate = GetCentralEuropeanDate(asOn);
        var today = GetCentralEuropeanDate(timeProvider.GetUtcNow());

        if (asOnDate == today)
        {
            return DailyRatesUrl;
        }

        if (asOnDate >= today.AddDays(-90))
        {
            return Last90DaysRatesUrl;
        }

        return Since1999RatesUrl;
    }

    private async Task<XDocument> GetExchangeRateDocumentAsync(Uri requestURL, CancellationToken cancellationToken)
    {
        var cacheKey = (requestURL, GetCentralEuropeanDate(this.timeProvider.GetUtcNow()));

        await ExchangeRateDocumentSemaphore.WaitAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            if (ExchangeRateDocuments.TryGetValue(cacheKey, out var cachedDocument))
            {
                return cachedDocument;
            }

            var document = await this.GetExchangeRateDocumentCoreAsync(requestURL, cancellationToken)
                .ConfigureAwait(false);
            ExchangeRateDocuments[cacheKey] = document;

            return document;
        }
        finally
        {
            _ = ExchangeRateDocumentSemaphore.Release();
        }
    }

    private async Task<XDocument> GetExchangeRateDocumentCoreAsync(
        Uri requestURL,
        CancellationToken cancellationToken)
    {
        var responseStream = await this.httpClient.GetStreamAsync(requestURL, cancellationToken).ConfigureAwait(false);

        await using (responseStream.ConfigureAwait(false))
        {
            return XDocument.Load(responseStream);
        }
    }

    private void VerifyDate(DateTimeOffset asOn)
    {
        if (asOn > this.timeProvider.GetUtcNow())
        {
            throw new ArgumentException("Exchange rate forecasting are not supported.", nameof(asOn));
        }
    }
}
