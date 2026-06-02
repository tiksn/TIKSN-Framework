namespace TIKSN.Finance.ForeignExchange;

public record class ExchangeRate(CurrencyPair Pair, DateTimeOffset AsOn, decimal Rate)
{
    public CurrencyPair Pair { get; init; } = Pair ?? throw new ArgumentNullException(nameof(Pair));

    public decimal Rate { get; init; } = Rate > decimal.Zero
        ? Rate
        : throw new ArgumentOutOfRangeException(nameof(Rate), Rate, "Rate must be a positive number.");

    public ExchangeRate Reverse() => new(this.Pair.Reverse(), this.AsOn, decimal.One / this.Rate);
}
