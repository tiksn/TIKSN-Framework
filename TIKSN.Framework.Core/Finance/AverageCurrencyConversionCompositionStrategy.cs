using TIKSN.Finance.Helpers;

namespace TIKSN.Finance;

public class AverageCurrencyConversionCompositionStrategy : ICurrencyConversionCompositionStrategy
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

        var amounts = new List<decimal>();

        foreach (var converter in filteredConverters)
        {
            var convertedMoney =
                await converter.ConvertCurrencyAsync(baseMoney, counterCurrency, asOn, cancellationToken).ConfigureAwait(false);

            if (convertedMoney.Currency != counterCurrency)
            {
                throw new InvalidOperationException("Converted into wrong currency.");
            }

            amounts.Add(convertedMoney.Amount);
        }

        var amount = amounts.Average();

        return new Money(counterCurrency, amount);
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

        var rates = new List<decimal>();

        foreach (var converter in filteredConverters)
        {
            var rate = await converter.GetExchangeRateAsync(pair, asOn, cancellationToken).ConfigureAwait(false);

            rates.Add(rate);
        }

        return rates.Average();
    }
}
