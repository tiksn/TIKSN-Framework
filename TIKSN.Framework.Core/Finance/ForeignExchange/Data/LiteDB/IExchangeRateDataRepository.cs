using TIKSN.Data.LiteDB;

namespace TIKSN.Finance.ForeignExchange.Data.LiteDB
{
    public interface IExchangeRateDataRepository : ILiteDbRepository<ExchangeRateDataEntity, Guid>
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
}
