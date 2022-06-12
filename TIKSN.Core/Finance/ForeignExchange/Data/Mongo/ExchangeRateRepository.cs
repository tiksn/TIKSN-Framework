using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MongoDB.Driver;
using TIKSN.Data.Mongo;

namespace TIKSN.Finance.ForeignExchange.Data.Mongo
{
    public class ExchangeRateRepository : MongoRepository<ExchangeRateEntity, Guid>, IExchangeRateRepository
    {
        public ExchangeRateRepository(
            IMongoClientSessionProvider mongoClientSessionProvider,
            IMongoDatabaseProvider mongoDatabaseProvider) : base(
                mongoClientSessionProvider,
                mongoDatabaseProvider,
                "ExchangeRates")
        {
        }

        public Task<ExchangeRateEntity> GetOrDefaultAsync(
            Guid foreignExchangeID,
            string baseCurrencyCode,
            string counterCurrencyCode,
            DateTimeOffset asOn,
            CancellationToken cancellationToken)
        {
            var filter = Builders<ExchangeRateEntity>.Filter.And(
                Builders<ExchangeRateEntity>.Filter.Eq(item => item.ForeignExchangeID, foreignExchangeID),
                Builders<ExchangeRateEntity>.Filter.Eq(item => item.BaseCurrencyCode, baseCurrencyCode),
                Builders<ExchangeRateEntity>.Filter.Eq(item => item.CounterCurrencyCode, counterCurrencyCode),
                Builders<ExchangeRateEntity>.Filter.Eq(item => item.AsOn, asOn));

            return base.SingleOrDefaultAsync(filter, cancellationToken);
        }

        public Task<IReadOnlyCollection<ExchangeRateEntity>> SearchAsync(
            Guid foreignExchangeID,
            string baseCurrencyCode,
            string counterCurrencyCode,
            DateTimeOffset dateFrom,
            DateTimeOffset dateTo,
            CancellationToken cancellationToken)
        {
            var filter = Builders<ExchangeRateEntity>.Filter.And(
                Builders<ExchangeRateEntity>.Filter.Eq(item => item.ForeignExchangeID, foreignExchangeID),
                Builders<ExchangeRateEntity>.Filter.Eq(item => item.BaseCurrencyCode, baseCurrencyCode),
                Builders<ExchangeRateEntity>.Filter.Eq(item => item.CounterCurrencyCode, counterCurrencyCode),
                Builders<ExchangeRateEntity>.Filter.Gte(item => item.AsOn, dateFrom),
                Builders<ExchangeRateEntity>.Filter.Lte(item => item.AsOn, dateTo));

            return base.SearchAsync(filter, cancellationToken);
        }
    }
}
