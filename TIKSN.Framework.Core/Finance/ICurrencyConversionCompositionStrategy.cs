namespace TIKSN.Finance;

public interface ICurrencyConversionCompositionStrategy
{
    public Task<Money> ConvertCurrencyAsync(
        Money baseMoney,
        IEnumerable<ICurrencyConverter> converters,
        CurrencyInfo counterCurrency,
        DateTimeOffset asOn,
        CancellationToken cancellationToken);

    public Task<decimal> GetExchangeRateAsync(
        IEnumerable<ICurrencyConverter> converters,
        CurrencyPair pair,
        DateTimeOffset asOn,
        CancellationToken cancellationToken);
}
