namespace TIKSN.Finance;

public class CompositeCrossCurrencyConverter : CompositeCurrencyConverter
{
    public CompositeCrossCurrencyConverter(ICurrencyConversionCompositionStrategy compositionStrategy)
        : base(compositionStrategy)
    {
    }

    public override async Task<Money> ConvertCurrencyAsync(Money baseMoney, CurrencyInfo counterCurrency,
        DateTimeOffset asOn, CancellationToken cancellationToken) =>
        await this.CompositionStrategy.ConvertCurrencyAsync(baseMoney, this.Converters, counterCurrency, asOn,
            cancellationToken).ConfigureAwait(false);

    public override async Task<IReadOnlyCollection<CurrencyPair>> GetCurrencyPairsAsync(DateTimeOffset asOn,
        CancellationToken cancellationToken)
    {
        var pairs = new HashSet<CurrencyPair>();

        foreach (var converter in this.Converters)
        {
            var currentPairs = await converter.GetCurrencyPairsAsync(asOn, cancellationToken).ConfigureAwait(false);

            foreach (var currentPair in currentPairs)
            {
                _ = pairs.Add(currentPair);
            }
        }

        return pairs;
    }

    public override async Task<decimal> GetExchangeRateAsync(CurrencyPair pair, DateTimeOffset asOn,
        CancellationToken cancellationToken) =>
        await this.CompositionStrategy.GetExchangeRateAsync(this.Converters, pair, asOn, cancellationToken).ConfigureAwait(false);
}
