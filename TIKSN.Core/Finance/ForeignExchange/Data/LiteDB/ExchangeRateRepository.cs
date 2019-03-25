﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TIKSN.Data.LiteDB;

namespace TIKSN.Finance.ForeignExchange.Data.LiteDB
{
    public class ExchangeRateRepository : LiteDbRepository<ExchangeRateEntity, int>, IExchangeRateRepository
    {
        public ExchangeRateRepository(ILiteDbDatabaseProvider databaseProvider) : base(databaseProvider, "ExchangeRates")
        {
        }

        public Task<int> GetMaximalIdAsync(CancellationToken cancellationToken)
        {
            var maxValue = collection.Max(x => x.ID);

            if (maxValue.IsInt32)
                return Task.FromResult(maxValue.AsInt32);

            return Task.FromResult(0);
        }

        public Task<ExchangeRateEntity> GetOrDefaultAsync(int foreignExchangeID, string baseCurrencyCode, string counterCurrencyCode, DateTimeOffset asOn, CancellationToken cancellationToken)
        {
            var result = collection.FindOne(x => x.ForeignExchangeID == foreignExchangeID && x.BaseCurrencyCode == baseCurrencyCode && x.CounterCurrencyCode == counterCurrencyCode && x.AsOn == asOn);

            return Task.FromResult(result);
        }

        public Task<IReadOnlyCollection<ExchangeRateEntity>> SearchAsync(int foreignExchangeID, string baseCurrencyCode, string counterCurrencyCode, DateTimeOffset dateFrom, DateTimeOffset dateTo, CancellationToken cancellationToken)
        {
            var results = collection.Find(x => x.ForeignExchangeID == foreignExchangeID && x.BaseCurrencyCode == baseCurrencyCode && x.CounterCurrencyCode == counterCurrencyCode && x.AsOn >= dateFrom && x.AsOn <= dateTo);

            return Task.FromResult<IReadOnlyCollection<ExchangeRateEntity>>(results.ToArray());
        }
    }
}