namespace TIKSN.Finance.ForeignExchange;

public interface IExchangeRatesProvider
{
    Task<IEnumerable<ExchangeRate>> GetExchangeRatesAsync(DateTimeOffset asOn, CancellationToken cancellationToken);
}
