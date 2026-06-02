using System.Globalization;
using System.Text;
using System.Xml.Linq;
using TIKSN.Globalization;

namespace TIKSN.Finance.ForeignExchange.Bank;

public class FederalReserveSystem : IFederalReserveSystem
{
    private static readonly CompositeFormat DataUrlFormat =
        CompositeFormat.Parse(
            "https://www.federalreserve.gov/datadownload/Output.aspx?rel=H10&series=60f32914ab61dfab590e0e470153e3ae&lastObs=&from={0}&to={1}&filetype=sdmx&label=include&layout=seriescolumn");

    private static readonly CultureInfo EnglishUnitedStates = new("en-US");
    private static readonly Dictionary<Uri, IReadOnlyCollection<ExchangeRate>> ExchangeRatesCache = [];
    private static readonly SemaphoreSlim ExchangeRatesSemaphore = new(initialCount: 1, maxCount: 1);
    private static readonly TimeZoneInfo FederalReserveTimeZone = FindFederalReserveTimeZone();
    private static readonly CurrencyInfo UnitedStatesDollar = new(new RegionInfo("en-US"));
    private readonly ICurrencyFactory currencyFactory;
    private readonly HttpClient httpClient;
    private readonly TimeProvider timeProvider;

    public FederalReserveSystem(
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

        return new Money(counterCurrency, rate * baseMoney.Amount);
    }

    public async Task<IReadOnlyCollection<CurrencyPair>> GetCurrencyPairsAsync(
        DateTimeOffset asOn,
        CancellationToken cancellationToken)
    {
        ValidateDate(asOn, this.timeProvider);

        var result = new List<CurrencyPair>();

        var rates = await this.GetRatesAsync(asOn, cancellationToken).ConfigureAwait(false);

        foreach (var someCurrency in rates.Keys)
        {
            result.Add(new CurrencyPair(UnitedStatesDollar, someCurrency));
            result.Add(new CurrencyPair(someCurrency, UnitedStatesDollar));
        }

        return result;
    }

    public async Task<decimal> GetExchangeRateAsync(
        CurrencyPair pair,
        DateTimeOffset asOn,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(pair);

        ValidateDate(asOn, this.timeProvider);

        var rates = await this.GetRatesAsync(asOn, cancellationToken).ConfigureAwait(false);

        if (pair.BaseCurrency == UnitedStatesDollar)
        {
            if (rates.TryGetValue(pair.CounterCurrency, out var rate))
            {
                return rate;
            }
        }
        else if (pair.CounterCurrency == UnitedStatesDollar &&
                 rates.TryGetValue(pair.BaseCurrency, out var counterRate))
        {
            return decimal.One / counterRate;
        }

        throw new ArgumentException($"Currency pair '{pair}' not supported.", nameof(pair));
    }

#pragma warning disable MA0051 // Method is too long

    public async Task<IReadOnlyCollection<ExchangeRate>> GetExchangeRatesAsync(
        DateTimeOffset asOn,
        CancellationToken cancellationToken)
#pragma warning restore MA0051 // Method is too long
    {
        var requestedDate = TimeZoneInfo.ConvertTime(asOn, FederalReserveTimeZone);
        var dataUrl = new Uri(string.Format(EnglishUnitedStates, DataUrlFormat,
            requestedDate.AddDays(-10d).ToString("MM/dd/yyyy", EnglishUnitedStates),
            requestedDate.ToString("MM/dd/yyyy", EnglishUnitedStates)));

        await ExchangeRatesSemaphore.WaitAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            if (ExchangeRatesCache.TryGetValue(dataUrl, out var cachedRates))
            {
                return cachedRates;
            }

            var rates = await this.GetExchangeRatesCoreAsync(dataUrl, cancellationToken).ConfigureAwait(false);
            ExchangeRatesCache[dataUrl] = rates;

            return rates;
        }
        finally
        {
            _ = ExchangeRatesSemaphore.Release();
        }
    }

    private static TimeZoneInfo FindFederalReserveTimeZone()
    {
        if (TimeZoneInfo.TryFindSystemTimeZoneById("America/New_York", out var ianaTimeZone))
        {
            return ianaTimeZone;
        }

        return TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
    }

    private static bool IsCurrencyPerUsdSeries(XElement? seriesElement)
    {
        if (seriesElement is null)
        {
            return false;
        }

        if (string.Equals(seriesElement.Attribute("UNIT")?.Value, "Currency:_Per_USD",
                StringComparison.OrdinalIgnoreCase))
        {
            return true;
        }

        var seriesName = seriesElement.Attribute("SERIES_NAME")?.Value;

        if (!string.IsNullOrWhiteSpace(seriesName))
        {
            return seriesName.StartsWith("RXI_", StringComparison.OrdinalIgnoreCase);
        }

        return false;
    }

    private static void ValidateDate(DateTimeOffset asOn, TimeProvider timeProvider)
    {
        if (asOn > timeProvider.GetUtcNow())
        {
            throw new ArgumentException("Exchange rate forecasting are not supported.", nameof(asOn));
        }
    }

    private async Task<IReadOnlyCollection<ExchangeRate>> GetExchangeRatesCoreAsync(
        Uri dataUrl,
        CancellationToken cancellationToken)
    {
        var response = await this.httpClient.GetAsync(dataUrl, cancellationToken).ConfigureAwait(false);
        using (response)
        {
            _ = response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);

            if (string.IsNullOrWhiteSpace(content))
            {
                throw new InvalidOperationException(
                    string.Create(CultureInfo.InvariantCulture,
                        $"Federal Reserve returned an empty exchange rate document from {dataUrl}."));
            }

            var xdoc = XDocument.Parse(content);

            var result = new List<ExchangeRate>();

            foreach (var seriesElement in xdoc
                         ?.Element("{http://www.SDMX.org/resources/SDMXML/schemas/v1_0/message}MessageGroup")
                         ?.Element("{http://www.federalreserve.gov/structure/compact/common}DataSet")
                         ?.Elements("{http://www.federalreserve.gov/structure/compact/H10_H10}Series") ?? [])
            {
                var currencyCode = seriesElement?.Attribute("CURRENCY")?.Value;
                var fx = seriesElement?.Attribute("FX")?.Value;

                if (!string.Equals(currencyCode, "NA", StringComparison.Ordinal)
                    && currencyCode is not null
                    && fx is not null)
                {
                    var rates = new Dictionary<DateTimeOffset, decimal>();

                    foreach (var obsElement in seriesElement?.Elements(
                                 "{http://www.federalreserve.gov/structure/compact/common}Obs") ?? [])
                    {
                        var obsStatus = obsElement?.Attribute("OBS_STATUS")?.Value;
                        if (!string.Equals(obsStatus, "ND", StringComparison.Ordinal))
                        {
                            var obsValue = decimal.Parse(obsElement?.Attribute("OBS_VALUE")?.Value ?? string.Empty,
                                EnglishUnitedStates);
                            var period = DateTimeOffset.Parse(
                                obsElement?.Attribute("TIME_PERIOD")?.Value ?? string.Empty,
                                EnglishUnitedStates);

                            var obsValueRate = IsCurrencyPerUsdSeries(seriesElement)
                                ? obsValue
                                : decimal.One / obsValue;

                            rates.Add(period, obsValueRate);
                        }
                    }

                    var date = rates.Keys.Max();
                    var rate = rates[date];

                    if (string.Equals(fx, "ZAL", StringComparison.Ordinal))
                    {
                        result.Add(new ExchangeRate(
                            new CurrencyPair(UnitedStatesDollar, this.currencyFactory.Create(currencyCode)), date,
                            rate));
                    }
                    else if (string.Equals(fx, "VEB", StringComparison.Ordinal))
                    {
                        result.Add(new ExchangeRate(
                            new CurrencyPair(UnitedStatesDollar, this.currencyFactory.Create("VEF")), date, rate));
                    }
                    else
                    {
                        result.Add(new ExchangeRate(
                            new CurrencyPair(UnitedStatesDollar, this.currencyFactory.Create(fx)), date, rate));
                    }
                }
            }

            return result;
        }
    }

    private async Task<Dictionary<CurrencyInfo, decimal>> GetRatesAsync(
        DateTimeOffset asOn,
        CancellationToken cancellationToken)
    {
        var rates = await this.GetExchangeRatesAsync(asOn, cancellationToken).ConfigureAwait(false);
        var result = new Dictionary<CurrencyInfo, decimal>();

        foreach (var rawRate in rates)
        {
            result.Add(rawRate.Pair.CounterCurrency, rawRate.Rate);
        }

        return result;
    }
}
