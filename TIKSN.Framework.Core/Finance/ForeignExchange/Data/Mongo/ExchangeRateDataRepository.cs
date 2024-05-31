using MongoDB.Driver;
using TIKSN.Data.Mongo;

namespace TIKSN.Finance.ForeignExchange.Data.Mongo;

public class ExchangeRateDataRepository : MongoRepository<ExchangeRateDataEntity, Guid>, IExchangeRateDataRepository
{
    public ExchangeRateDataRepository(
        IMongoClientSessionProvider mongoClientSessionProvider,
        IMongoDatabaseProvider mongoDatabaseProvider) : base(
            mongoClientSessionProvider,
            mongoDatabaseProvider,
            "ExchangeRates")
    {
    }

    protected override SortDefinition<ExchangeRateDataEntity> PageSortDefinition
        => Builders<ExchangeRateDataEntity>.Sort.Ascending(x => x.AsOn);

    public Task<IReadOnlyList<ExchangeRateDataEntity>> SearchAsync(
        string baseCurrencyCode,
        string counterCurrencyCode,
        DateTime dateFrom,
        DateTime dateTo,
        CancellationToken cancellationToken)
    {
        var filter = Builders<ExchangeRateDataEntity>.Filter.And(
            Builders<ExchangeRateDataEntity>.Filter.Eq(item => item.BaseCurrencyCode, baseCurrencyCode),
            Builders<ExchangeRateDataEntity>.Filter.Eq(item => item.CounterCurrencyCode, counterCurrencyCode),
            Builders<ExchangeRateDataEntity>.Filter.Gte(item => item.AsOn, dateFrom),
            Builders<ExchangeRateDataEntity>.Filter.Lte(item => item.AsOn, dateTo));

        return this.SearchAsync(filter, cancellationToken);
    }

    public Task<IReadOnlyList<ExchangeRateDataEntity>> SearchAsync(
        Guid foreignExchangeID,
        string baseCurrencyCode,
        string counterCurrencyCode,
        DateTime dateFrom,
        DateTime dateTo,
        CancellationToken cancellationToken)
    {
        var filter = Builders<ExchangeRateDataEntity>.Filter.And(
            Builders<ExchangeRateDataEntity>.Filter.Eq(item => item.ForeignExchangeID, foreignExchangeID),
            Builders<ExchangeRateDataEntity>.Filter.Eq(item => item.BaseCurrencyCode, baseCurrencyCode),
            Builders<ExchangeRateDataEntity>.Filter.Eq(item => item.CounterCurrencyCode, counterCurrencyCode),
            Builders<ExchangeRateDataEntity>.Filter.Gte(item => item.AsOn, dateFrom),
            Builders<ExchangeRateDataEntity>.Filter.Lte(item => item.AsOn, dateTo));

        return this.SearchAsync(filter, cancellationToken);
    }
}
