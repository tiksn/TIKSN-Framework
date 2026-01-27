using TIKSN.Data;

namespace TIKSN.Finance.ForeignExchange.Data.EntityFrameworkCore;

public interface IExchangeRateDataRepository
    : IRepository<ExchangeRateDataEntity>
    , IQueryRepository<ExchangeRateDataEntity, Guid>
    , IStreamRepository<ExchangeRateDataEntity>
{
    public Task<IReadOnlyList<ExchangeRateDataEntity>> SearchAsync(
        string baseCurrencyCode,
        string counterCurrencyCode,
        DateTime dateFrom,
        DateTime dateTo,
        CancellationToken cancellationToken);

    public Task<IReadOnlyList<ExchangeRateDataEntity>> SearchAsync(
        Guid foreignExchangeID,
        string baseCurrencyCode,
        string counterCurrencyCode,
        DateTime dateFrom,
        DateTime dateTo,
        CancellationToken cancellationToken);
}
