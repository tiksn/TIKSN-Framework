using TIKSN.Data.Mongo;

namespace TIKSN.Finance.ForeignExchange.Data.Mongo;

public interface IExchangeRateDataRepository : IMongoRepository<ExchangeRateDataEntity, Guid>
{
    Task<IReadOnlyList<ExchangeRateDataEntity>> SearchAsync(
        string baseCurrencyCode,
        string counterCurrencyCode,
        DateTime dateFrom,
        DateTime dateTo,
        CancellationToken cancellationToken);

    Task<IReadOnlyList<ExchangeRateDataEntity>> SearchAsync(
        Guid foreignExchangeID,
        string baseCurrencyCode,
        string counterCurrencyCode,
        DateTime dateFrom,
        DateTime dateTo,
        CancellationToken cancellationToken);
}
