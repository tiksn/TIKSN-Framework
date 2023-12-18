namespace TIKSN.Finance;

public interface ICurrencyConversionCompositionStrategy
{
    Task<Money> ConvertCurrencyAsync(
        Money baseMoney,
        IEnumerable<ICurrencyConverter> converters,
        CurrencyInfo counterCurrency,
        DateTimeOffset asOn,
        CancellationToken cancellationToken);

    Task<decimal> GetExchangeRateAsync(
        IEnumerable<ICurrencyConverter> converters,
        CurrencyPair pair,
        DateTimeOffset asOn,
        CancellationToken cancellationToken);
}
