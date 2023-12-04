namespace TIKSN.Finance.Cache;

public sealed class MemoryCachedCurrencyConverterEntry
{
    private MemoryCachedCurrencyConverterEntry(
        MemoryCachedCurrencyConverterEntryKind kind,
        IEnumerable<CurrencyPair> currencyPairs,
        decimal exchangeRate)
    {
        this.Kind = kind;
        this.CurrencyPairs = currencyPairs;
        this.ExchangeRate = exchangeRate;
    }

    public IEnumerable<CurrencyPair> CurrencyPairs { get; }

    public decimal ExchangeRate { get; }

    public MemoryCachedCurrencyConverterEntryKind Kind { get; }

    public static MemoryCachedCurrencyConverterEntry
        CreateForCurrencyPairs(IEnumerable<CurrencyPair> currencyPairs) =>
        new(MemoryCachedCurrencyConverterEntryKind.CurrencyPairs,
            currencyPairs, decimal.Zero);

    public static MemoryCachedCurrencyConverterEntry CreateForExchangeRate(decimal exchangeRate) =>
        new(MemoryCachedCurrencyConverterEntryKind.CurrencyPairs,
currencyPairs: null, exchangeRate);
}
