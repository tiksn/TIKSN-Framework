namespace TIKSN.Finance.Helpers;

internal static class CurrencyHelper
{
    public static Task<IEnumerable<ICurrencyConverter>> FilterConvertersAsync(
        IEnumerable<ICurrencyConverter> converters, CurrencyPair pair, DateTimeOffset asOn,
        CancellationToken cancellationToken) => FilterConvertersAsync(converters, pair.BaseCurrency,
        pair.CounterCurrency, asOn, cancellationToken);

    public static async Task<IEnumerable<ICurrencyConverter>> FilterConvertersAsync(
        IEnumerable<ICurrencyConverter> converters, CurrencyInfo baseCurrency, CurrencyInfo counterCurrency,
        DateTimeOffset asOn, CancellationToken cancellationToken)
    {
        var filteredConverters = new List<ICurrencyConverter>();

        foreach (var converter in converters)
        {
            var pairs = await converter.GetCurrencyPairsAsync(asOn, cancellationToken).ConfigureAwait(false);

            if (pairs.Any(item => item.BaseCurrency == baseCurrency && item.CounterCurrency == counterCurrency))
            {
                filteredConverters.Add(converter);
            }
        }

        return filteredConverters;
    }
}
