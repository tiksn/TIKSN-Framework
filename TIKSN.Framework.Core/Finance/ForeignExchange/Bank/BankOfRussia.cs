using System.Globalization;
using System.Text;
using System.Xml.Linq;
using TIKSN.Globalization;

namespace TIKSN.Finance.ForeignExchange.Bank;

public class BankOfRussia : IBankOfRussia
{
    private static readonly CompositeFormat AddressFormat =
        CompositeFormat.Parse("https://www.cbr.ru/scripts/XML_daily.asp?date_req={0:00}.{1:00}.{2}");

    private static readonly Dictionary<DateOnly, IReadOnlyCollection<PublishedRate>> PublishedRatesCache = [];

    private static readonly Lock PublishedRatesCacheLock = new();
    private static readonly SemaphoreSlim PublishedRatesSemaphore = new(initialCount: 1, maxCount: 1);

    private static readonly CurrencyInfo RussianRuble = new(new RegionInfo("ru-RU"));
    private static readonly CultureInfo RussianRussia = new("ru-RU");
    private readonly ICurrencyFactory currencyFactory;
    private readonly HttpClient httpClient;
    private readonly Dictionary<CurrencyInfo, decimal> rates;
    private readonly TimeProvider timeProvider;
    private DateTimeOffset? published;

    public BankOfRussia(
        HttpClient httpClient,
        ICurrencyFactory currencyFactory,
        TimeProvider timeProvider)
    {
        this.rates = [];
        this.published = null;
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

        await this.FetchOnDemandAsync(asOn, cancellationToken).ConfigureAwait(false);

        var rate = this.GetRate(baseMoney.Currency, counterCurrency);

        return new Money(counterCurrency, baseMoney.Amount * rate);
    }

    public async Task<IReadOnlyCollection<CurrencyPair>> GetCurrencyPairsAsync(
        DateTimeOffset asOn,
        CancellationToken cancellationToken)
    {
        await this.FetchOnDemandAsync(asOn, cancellationToken).ConfigureAwait(false);

        var pairs = new List<CurrencyPair>();

        foreach (var foreignCurrency in this.rates.Keys)
        {
            pairs.Add(new CurrencyPair(foreignCurrency, RussianRuble));
            pairs.Add(new CurrencyPair(RussianRuble, foreignCurrency));
        }

        return pairs;
    }

    public async Task<decimal> GetExchangeRateAsync(
        CurrencyPair pair,
        DateTimeOffset asOn,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(pair);

        await this.FetchOnDemandAsync(asOn, cancellationToken).ConfigureAwait(false);

        return this.GetRate(pair.BaseCurrency, pair.CounterCurrency);
    }

    public async Task<IReadOnlyCollection<ExchangeRate>> GetExchangeRatesAsync(
        DateTimeOffset asOn,
        CancellationToken cancellationToken)
    {
        ValidateDate(asOn, this.timeProvider);

        var thatDay = asOn.Date;

        var address = new Uri(string.Format(RussianRussia, AddressFormat, thatDay.Day, thatDay.Month, thatDay.Year));

        var date = DateOnly.FromDateTime(thatDay);
        var publishedRates = GetCachedPublishedRates(date);

        if (publishedRates == null)
        {
            await PublishedRatesSemaphore.WaitAsync(cancellationToken).ConfigureAwait(false);
            try
            {
                publishedRates = GetCachedPublishedRates(date);
                if (publishedRates == null)
                {
                    publishedRates = await this.FetchPublishedRatesAsync(address, cancellationToken)
                        .ConfigureAwait(false);
                    CachePublishedRates(date, publishedRates);
                }
            }
            finally
            {
                _ = PublishedRatesSemaphore.Release();
            }
        }

        return this.ApplyPublishedRates(publishedRates, asOn);
    }

    private static void CachePublishedRates(
        DateOnly date,
        IReadOnlyCollection<PublishedRate> publishedRates)
    {
        lock (PublishedRatesCacheLock)
        {
            PublishedRatesCache[date] = publishedRates;
        }
    }

    private static IReadOnlyCollection<PublishedRate>? GetCachedPublishedRates(DateOnly date)
    {
        lock (PublishedRatesCacheLock)
        {
            return PublishedRatesCache.GetValueOrDefault(date);
        }
    }

    private static List<PublishedRate> ReadPublishedRates(Stream responseStream)
    {
#pragma warning disable SYSLIB0001 // Type or member is obsolete
        using var stream​Reader = new Stream​Reader(responseStream, Encoding.UTF7);
#pragma warning restore SYSLIB0001 // Type or member is obsolete

        var xdoc = XDocument.Load(stream​Reader);
        var publishedRates = new List<PublishedRate>();

        foreach (var valuteElement in xdoc
                     ?.Element("ValCurs")
                     ?.Elements("Valute") ?? [])
        {
            var charCodeElement = valuteElement.Element("CharCode");

            if (charCodeElement == null)
            {
                continue;
            }

            var code = charCodeElement.Value;

            if (string.Equals(code, "NULL", StringComparison.Ordinal))
            {
                continue;
            }

            var nominalElement = valuteElement.Element("Nominal");
            var valueElement = valuteElement.Element("Value");
            var value = decimal.Parse(valueElement?.Value ?? string.Empty, RussianRussia);
            var nominal = decimal.Parse(nominalElement?.Value ?? string.Empty, RussianRussia);

            publishedRates.Add(new PublishedRate(code, value / nominal));
        }

        return publishedRates;
    }

    private static void ValidateDate(DateTimeOffset asOn, TimeProvider timeProvider)
    {
        if (asOn > timeProvider.GetUtcNow())
        {
            throw new ArgumentException("Exchange rate forecasting not supported.", nameof(asOn));
        }
    }

    private List<ExchangeRate> ApplyPublishedRates(
        IReadOnlyCollection<PublishedRate> publishedRates,
        DateTimeOffset asOn)
    {
        var result = new List<ExchangeRate>();

        lock (this.rates)
        {
            this.rates.Clear();

            foreach (var publishedRate in publishedRates)
            {
                var currency = this.currencyFactory.Create(publishedRate.CurrencyCode);

                result.Add(new ExchangeRate(new CurrencyPair(currency, RussianRuble), asOn,
                    publishedRate.Rate));
                result.Add(new ExchangeRate(new CurrencyPair(RussianRuble, currency), asOn,
                    decimal.One / publishedRate.Rate));

                this.rates.Add(currency, publishedRate.Rate);
            }

            this.published = asOn;
        }

        return result;
    }

    private async Task FetchOnDemandAsync(DateTimeOffset asOn, CancellationToken cancellationToken)
    {
        ValidateDate(asOn, this.timeProvider);

        if (!this.published.HasValue || this.published.Value.Date != asOn.Date)
        {
            _ = await this.GetExchangeRatesAsync(asOn, cancellationToken).ConfigureAwait(false);
        }
    }

    private async Task<IReadOnlyCollection<PublishedRate>> FetchPublishedRatesAsync(
        Uri address,
        CancellationToken cancellationToken)
    {
        var responseStream = await this.httpClient.GetStreamAsync(address, cancellationToken).ConfigureAwait(false);
        await using (responseStream.ConfigureAwait(false))
        {
            return ReadPublishedRates(responseStream);
        }
    }

    private decimal GetRate(CurrencyInfo baseCurrency, CurrencyInfo counterCurrency)
    {
        if (baseCurrency == RussianRuble)
        {
            if (this.rates.TryGetValue(counterCurrency, out var counterRate))
            {
                return decimal.One / counterRate;
            }
        }
        else if (counterCurrency == RussianRuble && this.rates.TryGetValue(baseCurrency, out var rate))
        {
            return rate;
        }

        throw new InvalidOperationException("Currency pair not supported.");
    }

    private sealed record PublishedRate(string CurrencyCode, decimal Rate);
}
