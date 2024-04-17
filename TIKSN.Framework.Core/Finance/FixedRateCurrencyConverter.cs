namespace TIKSN.Finance;

public class FixedRateCurrencyConverter : ICurrencyConverter
{
    private readonly decimal rate;

    public FixedRateCurrencyConverter(CurrencyPair pair, decimal rate)
    {
        this.CurrencyPair = pair ?? throw new ArgumentNullException(nameof(pair));

        if (rate > decimal.Zero)
        {
            this.rate = rate;
        }
        else
        {
            throw new ArgumentException("Rate cannot be negative or zero.", nameof(rate));
        }
    }

    public CurrencyPair CurrencyPair { get; }

    public Task<Money> ConvertCurrencyAsync(
        Money baseMoney,
        CurrencyInfo counterCurrency,
        DateTimeOffset asOn,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(baseMoney);
        ArgumentNullException.ThrowIfNull(counterCurrency);

        var requiredPair = new CurrencyPair(baseMoney.Currency, counterCurrency);

        if (this.CurrencyPair == requiredPair)
        {
            return Task.FromResult(new Money(this.CurrencyPair.CounterCurrency, baseMoney.Amount * this.rate));
        }

        throw new ArgumentException("Unsupported currency pair.", nameof(counterCurrency));
    }

    public Task<IReadOnlyCollection<CurrencyPair>> GetCurrencyPairsAsync(
        DateTimeOffset asOn,
        CancellationToken cancellationToken)
    {
        IReadOnlyCollection<CurrencyPair> singleItemList = [this.CurrencyPair];

        return Task.FromResult(singleItemList);
    }

    public Task<decimal> GetExchangeRateAsync(
        CurrencyPair pair,
        DateTimeOffset asOn,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(pair);

        if (this.CurrencyPair == pair)
        {
            return Task.FromResult(this.rate);
        }

        throw new ArgumentException("Unsupported currency pair.", nameof(pair));
    }
}
