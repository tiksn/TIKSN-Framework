using System.Globalization;
using System.Xml.Linq;
using TIKSN.Globalization;

namespace TIKSN.Finance.ForeignExchange.Bank;

public class EuropeanCentralBank : IEuropeanCentralBank
{
    private static readonly Uri DailyRatesUrl = new("https://www.ecb.europa.eu/stats/eurofxref/eurofxref-daily.xml");

    private static readonly CurrencyInfo Euro = new(new RegionInfo("de-DE"));
    private static readonly Uri Last90DaysRatesUrl = new("https://www.ecb.europa.eu/stats/eurofxref/eurofxref-hist-90d.xml");
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

        var responseStream = await this.httpClient.GetStreamAsync(requestURL, cancellationToken).ConfigureAwait(false);

        var xdoc = XDocument.Load(responseStream);

        var groupsCubes = xdoc.Element("{http://www.gesmes.org/xml/2002-08-01}Envelope")
            .Element("{http://www.ecb.int/vocabulary/2002-08-01/eurofxref}Cube")
            .Elements("{http://www.ecb.int/vocabulary/2002-08-01/eurofxref}Cube");

        var groupCubes = groupsCubes
            .Select(x =>
                new Tuple<XElement, DateTimeOffset>(x, DateTimeOffset.Parse(x.Attribute("time").Value, CultureInfo.InvariantCulture)))
            .Where(z => z.Item2 <= asOn).OrderByDescending(y => y.Item2).First();

        var rates = new List<ExchangeRate>();

        foreach (var rateCube in groupCubes.Item1.Elements(
            "{http://www.ecb.int/vocabulary/2002-08-01/eurofxref}Cube"))
        {
            var currencyCode = rateCube.Attribute("currency").Value;
            var rate = decimal.Parse(rateCube.Attribute("rate").Value, CultureInfo.InvariantCulture);

            rates.Add(new ExchangeRate(new CurrencyPair(Euro, this.currencyFactory.Create(currencyCode)),
                groupCubes.Item2, rate));
        }

        return rates;
    }

    private static Uri GetRatesUrl(DateTimeOffset asOn, TimeProvider timeProvider)
    {
        if (asOn.Date == timeProvider.GetUtcNow().Date)
        {
            return DailyRatesUrl;
        }

        if (asOn.Date >= timeProvider.GetUtcNow().AddDays(-90).Date)
        {
            return Last90DaysRatesUrl;
        }

        return Since1999RatesUrl;
    }

    private void VerifyDate(DateTimeOffset asOn)
    {
        if (asOn > this.timeProvider.GetUtcNow())
        {
            throw new ArgumentException("Exchange rate forecasting are not supported.", nameof(asOn));
        }
    }
}
