using TIKSN.Finance.Helpers;

namespace TIKSN.Finance;

public class SingleEntryCurrencyConversionCompositionStrategy : ICurrencyConversionCompositionStrategy
{
    public async Task<Money> ConvertCurrencyAsync(
        Money baseMoney,
        IEnumerable<ICurrencyConverter> converters,
        CurrencyInfo counterCurrency,
        DateTimeOffset asOn,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(baseMoney);
        ArgumentNullException.ThrowIfNull(converters);
        ArgumentNullException.ThrowIfNull(counterCurrency);

        var filteredConverters = await CurrencyHelper.FilterConvertersAsync(converters, baseMoney.Currency,
            counterCurrency, asOn, cancellationToken).ConfigureAwait(false);

        var converter = filteredConverters.Single();

        return await converter.ConvertCurrencyAsync(baseMoney, counterCurrency, asOn, cancellationToken).ConfigureAwait(false);
    }

    public async Task<decimal> GetExchangeRateAsync(
        IEnumerable<ICurrencyConverter> converters,
        CurrencyPair pair,
        DateTimeOffset asOn,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(converters);
        ArgumentNullException.ThrowIfNull(pair);

        var filteredConverters = await CurrencyHelper.FilterConvertersAsync(converters, pair, asOn, cancellationToken).ConfigureAwait(false);

        var converter = filteredConverters.Single();

        return await converter.GetExchangeRateAsync(pair, asOn, cancellationToken).ConfigureAwait(false);
    }
}
