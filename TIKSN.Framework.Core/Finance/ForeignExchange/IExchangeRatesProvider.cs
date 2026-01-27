namespace TIKSN.Finance.ForeignExchange;

public interface IExchangeRatesProvider
{
    public Task<IReadOnlyCollection<ExchangeRate>> GetExchangeRatesAsync(
        DateTimeOffset asOn,
        CancellationToken cancellationToken);
}
