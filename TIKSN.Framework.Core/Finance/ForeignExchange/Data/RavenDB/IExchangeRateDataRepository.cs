using TIKSN.Data.RavenDB;

namespace TIKSN.Finance.ForeignExchange.Data.RavenDB;

public interface IExchangeRateDataRepository : IRavenRepository<ExchangeRateDataEntity, Guid>
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
