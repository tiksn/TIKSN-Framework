namespace TIKSN.Finance.Cache;

public sealed class MemoryCachedCurrencyConverterEntry
{
    private MemoryCachedCurrencyConverterEntry(
        MemoryCachedCurrencyConverterEntryKind kind,
        IReadOnlyCollection<CurrencyPair> currencyPairs,
        decimal exchangeRate)
    {
        this.Kind = kind;
        this.CurrencyPairs = currencyPairs;
        this.ExchangeRate = exchangeRate;
    }

    public IReadOnlyCollection<CurrencyPair> CurrencyPairs { get; }

    public decimal ExchangeRate { get; }

    public MemoryCachedCurrencyConverterEntryKind Kind { get; }

    public static MemoryCachedCurrencyConverterEntry
        CreateForCurrencyPairs(IReadOnlyCollection<CurrencyPair> currencyPairs) =>
        new(MemoryCachedCurrencyConverterEntryKind.CurrencyPairs,
            currencyPairs, decimal.Zero);

    public static MemoryCachedCurrencyConverterEntry CreateForExchangeRate(
        decimal exchangeRate) =>
        new(MemoryCachedCurrencyConverterEntryKind.CurrencyPairs, currencyPairs: [], exchangeRate);
}
