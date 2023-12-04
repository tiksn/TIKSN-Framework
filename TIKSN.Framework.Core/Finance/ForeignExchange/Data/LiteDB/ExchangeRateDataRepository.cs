using LiteDB;
using TIKSN.Data.LiteDB;

namespace TIKSN.Finance.ForeignExchange.Data.LiteDB;

public class ExchangeRateDataRepository : LiteDbRepository<ExchangeRateDataEntity, Guid>, IExchangeRateDataRepository
{
    public ExchangeRateDataRepository(ILiteDbDatabaseProvider databaseProvider) : base(databaseProvider,
        "ExchangeRates", x => new BsonValue(x))
    {
    }

    public Task<IReadOnlyList<ExchangeRateDataEntity>> SearchAsync(
        string baseCurrencyCode,
        string counterCurrencyCode,
        DateTime dateFrom,
        DateTime dateTo,
        CancellationToken cancellationToken)
    {
        var results = this.collection.Find(x =>
            x.BaseCurrencyCode == baseCurrencyCode &&
            x.CounterCurrencyCode == counterCurrencyCode && x.AsOn >= dateFrom && x.AsOn <= dateTo);

        return Task.FromResult<IReadOnlyList<ExchangeRateDataEntity>>(results.ToArray());
    }

    public Task<IReadOnlyList<ExchangeRateDataEntity>> SearchAsync(
        Guid foreignExchangeID,
        string baseCurrencyCode,
        string counterCurrencyCode,
        DateTime dateFrom,
        DateTime dateTo,
        CancellationToken cancellationToken)
    {
        var results = this.collection.Find(x =>
            x.ForeignExchangeID == foreignExchangeID && x.BaseCurrencyCode == baseCurrencyCode &&
            x.CounterCurrencyCode == counterCurrencyCode && x.AsOn >= dateFrom && x.AsOn <= dateTo);

        return Task.FromResult<IReadOnlyList<ExchangeRateDataEntity>>(results.ToArray());
    }
}
