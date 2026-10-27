using System.Globalization;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using TIKSN.Globalization;

namespace TIKSN.Finance.ForeignExchange.Bank;

public class ReserveBankOfNewZealand : IReserveBankOfNewZealand
{
    private const string DailyRates =
        "https://www.rbnz.govt.nz/-/media/project/sites/rbnz/files/statistics/series/b/b1/hb1-daily.xlsx";

    private static readonly TimeZoneInfo BankTimeZone =
        TimeZoneInfo.FindSystemTimeZoneById("New Zealand Standard Time");

    private static readonly Dictionary<string, string> CurrencyCodes = new(StringComparer.OrdinalIgnoreCase)
    {
        ["Australian dollar"] = "AUD",
        ["Canadian dollar"] = "CAD",
        ["Chinese renminbi"] = "CNY",
        ["Chinese yuan"] = "CNY",
        ["European euro"] = "EUR",
        ["Euro"] = "EUR",
        ["Hong Kong dollar"] = "HKD",
        ["Indian rupee"] = "INR",
        ["Indonesian rupiah"] = "IDR",
        ["Japanese yen"] = "JPY",
        ["Korean won"] = "KRW",
        ["Malaysian ringgit"] = "MYR",
        ["Philippine peso"] = "PHP",
        ["Singapore dollar"] = "SGD",
        ["South Korean won"] = "KRW",
        ["Taiwan dollar"] = "TWD",
        ["Thai baht"] = "THB",
        ["UK pound sterling"] = "GBP",
        ["United Kingdom pound sterling"] = "GBP",
        ["United States dollar"] = "USD",
        ["US dollar"] = "USD",
        ["Vietnamese dong"] = "VND",
    };

    private static readonly CurrencyInfo NewZealandDollar = new(new RegionInfo("en-NZ"));

    private static readonly Dictionary<(Uri WorkbookUrl, DateOnly RequestDate), IReadOnlyList<RawExchangeRate>>
        RawDataCache = [];

    private static readonly SemaphoreSlim RawDataSemaphore = new(initialCount: 1, maxCount: 1);
    private static readonly Uri WorkbookUrl = new(DailyRates);
    private readonly ICurrencyFactory currencyFactory;
    private readonly ICurrencyPairFactory currencyPairFactory;
    private readonly HttpClient httpClient;
    private readonly Dictionary<DateOnly, Dictionary<CurrencyInfo, decimal>> rates;
    private readonly TimeProvider timeProvider;
    private DateTimeOffset lastFetchDate;

    public ReserveBankOfNewZealand(
        HttpClient httpClient,
        ICurrencyFactory currencyFactory,
        ICurrencyPairFactory currencyPairFactory,
        TimeProvider timeProvider)
    {
        this.rates = [];
        this.lastFetchDate = DateTimeOffset.MinValue;
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

        var pair = this.currencyPairFactory.Create(baseMoney.Currency, counterCurrency);

        var rate = await this.GetExchangeRateAsync(pair, asOn, cancellationToken).ConfigureAwait(false);

        return new Money(counterCurrency, baseMoney.Amount * rate);
    }

    public async Task<IReadOnlyCollection<CurrencyPair>> GetCurrencyPairsAsync(
        DateTimeOffset asOn,
        CancellationToken cancellationToken)
    {
        await this.FetchOnDemandAsync(cancellationToken).ConfigureAwait(false);

        this.VerifyDate(asOn);

        var pairs = new List<CurrencyPair>();

        foreach (var foreignCurrency in this.GetRatesByDate(asOn).Keys)
        {
            pairs.Add(this.currencyPairFactory.Create(NewZealandDollar, foreignCurrency));
            pairs.Add(this.currencyPairFactory.Create(foreignCurrency, NewZealandDollar));
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

        this.VerifyDate(asOn);

        if (pair.BaseCurrency == NewZealandDollar)
        {
            if (this.GetRatesByDate(asOn).TryGetValue(pair.CounterCurrency, out var rate))
            {
                return rate;
            }
        }
        else if (pair.CounterCurrency == NewZealandDollar
                 && this.GetRatesByDate(asOn).TryGetValue(pair.BaseCurrency, out var counterRate))
        {
            return decimal.One / counterRate;
        }

        throw new ArgumentException("Currency pair not supported.", nameof(pair));
    }

    public async Task<IReadOnlyCollection<ExchangeRate>> GetExchangeRatesAsync(
        DateTimeOffset asOn,
        CancellationToken cancellationToken)
    {
        this.VerifyDate(asOn);

        var rawData = await this.FetchRawDataAsync(WorkbookUrl, cancellationToken).ConfigureAwait(false);
        var result = new List<ExchangeRate>();

        lock (this.rates)
        {
            this.rates.Clear();

            foreach (var rawItem in rawData)
            {
                var currency = this.currencyFactory.Create(rawItem.CurrencyCode);

                if (!this.rates.TryGetValue(rawItem.AsOn, out var ratesByCurrency))
                {
                    ratesByCurrency = [];
                    this.rates.Add(rawItem.AsOn, ratesByCurrency);
                }

                ratesByCurrency[currency] = rawItem.Rate;
            }

            this.lastFetchDate = this.timeProvider.GetUtcNow();
        }

        var rateDate = this.GetAvailableRateDate(asOn);
        foreach (var rate in this.rates[rateDate])
        {
            result.Add(new ExchangeRate(
                this.currencyPairFactory.Create(NewZealandDollar, rate.Key),
                GetRateDateTimeOffset(rateDate),
                rate.Value));
        }

        return result;
    }

    private static DateTimeOffset ConvertToBankTimeZone(DateTimeOffset date) =>
        TimeZoneInfo.ConvertTime(date, BankTimeZone);

    private static int FindIndex(
        List<Dictionary<int, string>> items,
        Func<Dictionary<int, string>, bool> predicate)
    {
        for (var index = 0; index < items.Count; index++)
        {
            if (predicate(items[index]))
            {
                return index;
            }
        }

        return -1;
    }

    private static string? GetCellValue(
        Cell cell,
        SharedStringTable? sharedStringTable)
    {
        if (cell.DataType?.Value == CellValues.SharedString)
        {
            var sharedStringIndex = int.Parse(cell.CellValue?.Text ?? string.Empty, CultureInfo.InvariantCulture);
            return sharedStringTable?.ElementAt(sharedStringIndex).InnerText;
        }

        if (cell.DataType?.Value == CellValues.InlineString)
        {
            return cell.InlineString?.InnerText;
        }

        return cell.CellValue?.Text ?? cell.InnerText;
    }

    private static int GetColumnIndex(string cellReference)
    {
        var columnIndex = 0;

        foreach (var character in cellReference.TakeWhile(char.IsLetter))
        {
            columnIndex *= 26;
            columnIndex += char.ToUpperInvariant(character) - 'A' + 1;
        }

        return columnIndex;
    }

    private static DateOnly GetRateDate(DateTimeOffset asOn)
        => DateOnly.FromDateTime(ConvertToBankTimeZone(asOn).Date);

    private static DateTimeOffset GetRateDateTimeOffset(DateOnly asOnDate)
    {
        var dateTime = asOnDate.ToDateTime(TimeOnly.MinValue);
        return new DateTimeOffset(dateTime, BankTimeZone.GetUtcOffset(dateTime));
    }

    private static string NormalizeHeader(string value)
        => value.Replace(" ", string.Empty, StringComparison.Ordinal)
            .Replace("_", string.Empty, StringComparison.Ordinal)
            .ToUpperInvariant();

    private static List<RawExchangeRate> ParseWorkbook(Stream workbookStream)
    {
        using var spreadsheetDocument = SpreadsheetDocument.Open(workbookStream, isEditable: false);
        var workbookPart = spreadsheetDocument.WorkbookPart
            ?? throw new InvalidOperationException("Workbook part is missing.");
        var sharedStringTable = workbookPart.SharedStringTablePart?.SharedStringTable;

        var seriesDefinitionsRows = ReadSheetRows(workbookPart, "Series Definitions", sharedStringTable);
        var dataRows = ReadSheetRows(workbookPart, "Data", sharedStringTable);
        var seriesCurrencyCodes = ReadSeriesCurrencyCodes(seriesDefinitionsRows);

        return ReadExchangeRates(dataRows, seriesCurrencyCodes);
    }

    private static List<RawExchangeRate> ReadExchangeRates(
        List<Dictionary<int, string>> dataRows,
        Dictionary<string, string> seriesCurrencyCodes)
    {
        var headerIndex = FindIndex(dataRows, row =>
            row.Values.Any(value =>
                string.Equals(NormalizeHeader(value), "SERIESID", StringComparison.Ordinal) ||
                string.Equals(NormalizeHeader(value), "DATE", StringComparison.Ordinal)));

        if (headerIndex < 0)
        {
            throw new InvalidOperationException("Data sheet header row was not found.");
        }

        var header = dataRows[headerIndex];
        var currencyColumns = header
            .Where(item => item.Key > 1 && seriesCurrencyCodes.ContainsKey(item.Value))
            .ToDictionary(item => item.Key, item => seriesCurrencyCodes[item.Value]);

        var result = new List<RawExchangeRate>();

        foreach (var row in dataRows.Skip(headerIndex + 1))
        {
            if (!row.TryGetValue(1, out var rawDate) || !TryParseDate(rawDate, out var asOn))
            {
                continue;
            }

            foreach (var currencyColumn in currencyColumns)
            {
                if (row.TryGetValue(currencyColumn.Key, out var rawRate) &&
                    TryParseRate(rawRate, out var rate))
                {
                    result.Add(new RawExchangeRate(asOn, currencyColumn.Value, rate));
                }
            }
        }

        return result;
    }

    private static Dictionary<string, string> ReadSeriesCurrencyCodes(
        List<Dictionary<int, string>> seriesDefinitionsRows)
    {
        var headerIndex = FindIndex(seriesDefinitionsRows, row =>
            row.Values.Any(value => string.Equals(NormalizeHeader(value), "SERIESID", StringComparison.Ordinal)));

        if (headerIndex < 0)
        {
            throw new InvalidOperationException("Series Definitions sheet header row was not found.");
        }

        var header = seriesDefinitionsRows[headerIndex];
        var seriesIdColumn = header
            .Where(item => string.Equals(NormalizeHeader(item.Value), "SERIESID", StringComparison.Ordinal))
            .Select(item => item.Key)
            .First();
        var seriesColumn = header
            .Where(item => string.Equals(NormalizeHeader(item.Value), "SERIES", StringComparison.Ordinal))
            .Select(item => item.Key)
            .First();
        var result = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        foreach (var row in seriesDefinitionsRows.Skip(headerIndex + 1))
        {
            if (!row.TryGetValue(seriesIdColumn, out var seriesId) ||
                !row.TryGetValue(seriesColumn, out var seriesName) ||
                !TryGetCurrencyCode(seriesName, out var currencyCode))
            {
                continue;
            }

            result[seriesId] = currencyCode;
        }

        return result;
    }

    private static List<Dictionary<int, string>> ReadSheetRows(
        WorkbookPart workbookPart,
        string sheetName,
        SharedStringTable? sharedStringTable)
    {
        var workbook = workbookPart.Workbook
            ?? throw new InvalidOperationException("Workbook is missing.");
        var sheet = workbook.Sheets?.Elements<Sheet>()
                .FirstOrDefault(item => string.Equals(item.Name?.Value, sheetName, StringComparison.OrdinalIgnoreCase))
            ?? throw new InvalidOperationException($"Worksheet '{sheetName}' was not found.");
        var worksheetPart = (WorksheetPart)workbookPart.GetPartById(sheet.Id?.Value ?? string.Empty);
        var worksheet = worksheetPart.Worksheet
            ?? throw new InvalidOperationException($"Worksheet '{sheetName}' is empty.");
        var result = new List<Dictionary<int, string>>();

        foreach (var row in worksheet.Descendants<Row>())
        {
            var rowValues = new Dictionary<int, string>();

            foreach (var cell in row.Elements<Cell>())
            {
                var cellReference = cell.CellReference?.Value;
                if (string.IsNullOrWhiteSpace(cellReference))
                {
                    continue;
                }

                var cellValue = GetCellValue(cell, sharedStringTable);
                if (!string.IsNullOrWhiteSpace(cellValue))
                {
                    rowValues[GetColumnIndex(cellReference)] = cellValue.Trim();
                }
            }

            if (rowValues.Count > 0)
            {
                result.Add(rowValues);
            }
        }

        return result;
    }

    private static bool TryGetCurrencyCode(string seriesName, out string currencyCode)
    {
        if (CurrencyCodes.TryGetValue(seriesName.Trim(), out var exactCurrencyCode))
        {
            currencyCode = exactCurrencyCode;
            return true;
        }

        var matchedCurrency = CurrencyCodes.FirstOrDefault(currency =>
            seriesName.Contains(currency.Key, StringComparison.OrdinalIgnoreCase));
        if (matchedCurrency.Value is not null)
        {
            currencyCode = matchedCurrency.Value;
            return true;
        }

        currencyCode = string.Empty;
        return false;
    }

    private static bool TryParseDate(string value, out DateOnly date)
    {
        if (double.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out var serialDate) &&
            serialDate > 0d)
        {
            date = DateOnly.FromDateTime(DateTime.FromOADate(serialDate));
            return true;
        }

        if (DateTime.TryParse(value, CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal, out var dateTime))
        {
            date = DateOnly.FromDateTime(dateTime);
            return true;
        }

        if (DateTime.TryParse(value, CultureInfo.GetCultureInfo("en-NZ"), DateTimeStyles.AssumeLocal, out dateTime))
        {
            date = DateOnly.FromDateTime(dateTime);
            return true;
        }

        date = default;
        return false;
    }

    private static bool TryParseRate(string value, out decimal rate)
    {
        if (string.IsNullOrWhiteSpace(value) ||
            string.Equals(value, "..", StringComparison.Ordinal) ||
            string.Equals(value, "-", StringComparison.Ordinal))
        {
            rate = decimal.Zero;
            return false;
        }

        return decimal.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out rate);
    }

    private async Task FetchOnDemandAsync(CancellationToken cancellationToken)
    {
        if (this.timeProvider.GetUtcNow() - this.lastFetchDate > TimeSpan.FromDays(1d))
        {
            _ = await this.GetExchangeRatesAsync(this.timeProvider.GetUtcNow(), cancellationToken)
                .ConfigureAwait(false);
        }
    }

    private async Task<IReadOnlyList<RawExchangeRate>> FetchRawDataAsync(
        Uri workbookUrl,
        CancellationToken cancellationToken)
    {
        var cacheKey = (workbookUrl, GetRateDate(this.timeProvider.GetUtcNow()));

        await RawDataSemaphore.WaitAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            if (RawDataCache.TryGetValue(cacheKey, out var cachedRawData))
            {
                return cachedRawData;
            }

            var rawData = await this.FetchRawDataCoreAsync(workbookUrl, cancellationToken).ConfigureAwait(false);
            RawDataCache[cacheKey] = rawData;

            return rawData;
        }
        finally
        {
            _ = RawDataSemaphore.Release();
        }
    }

    private async Task<IReadOnlyList<RawExchangeRate>> FetchRawDataCoreAsync(
        Uri workbookUrl,
        CancellationToken cancellationToken)
    {
        var responseStream = await this.httpClient.GetStreamAsync(workbookUrl, cancellationToken).ConfigureAwait(false);

        await using (responseStream.ConfigureAwait(false))
        {
            var memoryStream = new MemoryStream();
            await using (memoryStream.ConfigureAwait(false))
            {
                await responseStream.CopyToAsync(memoryStream, cancellationToken).ConfigureAwait(false);
                memoryStream.Position = 0L;

                return ParseWorkbook(memoryStream);
            }
        }
    }

    private DateOnly GetAvailableRateDate(DateTimeOffset asOn)
    {
        var date = GetRateDate(asOn);

        if (this.rates.ContainsKey(date))
        {
            return date;
        }

        var latestAvailableDate = this.rates.Keys
            .Where(item => item <= date)
            .OrderDescending()
            .FirstOrDefault();

        if (latestAvailableDate != default)
        {
            return latestAvailableDate;
        }

        throw new ArgumentException("Exchange rate history not supported.", nameof(asOn));
    }

    private Dictionary<CurrencyInfo, decimal> GetRatesByDate(DateTimeOffset asOn)
        => this.rates[this.GetAvailableRateDate(asOn)];

    private void VerifyDate(DateTimeOffset asOn)
    {
        if (asOn > this.timeProvider.GetUtcNow())
        {
            throw new ArgumentException("Exchange rate forecasting not supported.", nameof(asOn));
        }
    }

    private sealed record RawExchangeRate(DateOnly AsOn, string CurrencyCode, decimal Rate);
}
