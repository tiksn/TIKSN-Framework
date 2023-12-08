namespace TIKSN.Finance;

public abstract class CompositeCurrencyConverter : ICompositeCurrencyConverter
{
    protected CompositeCurrencyConverter(ICurrencyConversionCompositionStrategy compositionStrategy)
    {
        this.CompositionStrategy =
            compositionStrategy ?? throw new ArgumentNullException(nameof(compositionStrategy));
        this.Converters = [];
    }

    protected ICurrencyConversionCompositionStrategy CompositionStrategy { get; }
    protected List<ICurrencyConverter> Converters { get; }

    public void Add(ICurrencyConverter converter) => this.Converters.Add(converter);

    public abstract Task<Money> ConvertCurrencyAsync(Money baseMoney, CurrencyInfo counterCurrency,
        DateTimeOffset asOn, CancellationToken cancellationToken);

    public abstract Task<IEnumerable<CurrencyPair>> GetCurrencyPairsAsync(DateTimeOffset asOn,
        CancellationToken cancellationToken);

    public abstract Task<decimal> GetExchangeRateAsync(CurrencyPair pair, DateTimeOffset asOn,
        CancellationToken cancellationToken);
}
