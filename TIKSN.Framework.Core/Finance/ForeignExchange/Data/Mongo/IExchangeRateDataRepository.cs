using TIKSN.Data.Mongo;

namespace TIKSN.Finance.ForeignExchange.Data.Mongo;

public interface IExchangeRateDataRepository : IMongoRepository<ExchangeRateDataEntity, Guid>
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
