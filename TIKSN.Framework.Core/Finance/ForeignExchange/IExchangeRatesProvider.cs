namespace TIKSN.Finance.ForeignExchange;

public interface IExchangeRatesProvider
{
    Task<IReadOnlyCollection<ExchangeRate>> GetExchangeRatesAsync(
        DateTimeOffset asOn,
        CancellationToken cancellationToken);
}
