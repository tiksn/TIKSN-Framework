namespace TIKSN.Finance.ForeignExchange;

public interface IExchangeRateProvider
{
    Task<ExchangeRate> GetExchangeRateAsync(CurrencyInfo baseCurrency, CurrencyInfo counterCurrency,
        DateTimeOffset asOn, CancellationToken cancellationToken);
}
