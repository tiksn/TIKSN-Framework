using TIKSN.Data;

namespace TIKSN.Finance.ForeignExchange.Data;

public interface IExchangeRateRepository
    : IRepository<ExchangeRateEntity>
    , IQueryRepository<ExchangeRateEntity, Guid>
    , IStreamRepository<ExchangeRateEntity>
{
    public Task<IReadOnlyList<ExchangeRateEntity>> SearchAsync(
        string baseCurrencyCode,
        string counterCurrencyCode,
        DateTime dateFrom,
        DateTime dateTo,
        CancellationToken cancellationToken);

    public Task<IReadOnlyList<ExchangeRateEntity>> SearchAsync(
        Guid foreignExchangeID,
        string baseCurrencyCode,
        string counterCurrencyCode,
        DateTime dateFrom,
        DateTime dateTo,
        CancellationToken cancellationToken);
}
