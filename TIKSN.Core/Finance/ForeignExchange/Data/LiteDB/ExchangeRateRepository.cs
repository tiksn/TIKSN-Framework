using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LiteDB;
using TIKSN.Data.LiteDB;

namespace TIKSN.Finance.ForeignExchange.Data.LiteDB
{
    public class ExchangeRateRepository : LiteDbRepository<ExchangeRateEntity, Guid>, IExchangeRateRepository
    {
        public ExchangeRateRepository(ILiteDbDatabaseProvider databaseProvider) : base(databaseProvider,
            "ExchangeRates", x => new BsonValue(x))
        {
        }

        public Task<IReadOnlyCollection<ExchangeRateEntity>> SearchAsync(
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

            return Task.FromResult<IReadOnlyCollection<ExchangeRateEntity>>(results.ToArray());
        }
    }
}
