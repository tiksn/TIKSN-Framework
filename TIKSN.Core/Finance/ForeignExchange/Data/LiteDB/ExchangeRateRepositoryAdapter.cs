using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TIKSN.Data;
using TIKSN.Data.LiteDB;

namespace TIKSN.Finance.ForeignExchange.Data.LiteDB
{
    public class ExchangeRateRepositoryAdapter
        : LiteDbRepositoryAdapter<ExchangeRateEntity, Guid, ExchangeRateDataEntity, Guid>
        , IExchangeRateRepository
    {
        protected readonly IExchangeRateDataRepository dataRepository;

        public ExchangeRateRepositoryAdapter(
            IExchangeRateDataRepository dataRepository,
            IMapper<ExchangeRateEntity, ExchangeRateDataEntity> domainEntityToDataEntityMapper,
            IMapper<ExchangeRateDataEntity, ExchangeRateEntity> dataEntityToDomainEntityMapper) : base(
                domainEntityToDataEntityMapper,
                dataEntityToDomainEntityMapper,
                IdentityMapper<Guid>.Instance,
                IdentityMapper<Guid>.Instance,
                dataRepository)
        {
            this.dataRepository = dataRepository ?? throw new ArgumentNullException(nameof(dataRepository));
        }

        public Task<IReadOnlyList<ExchangeRateEntity>> SearchAsync(
            Guid foreignExchangeID,
            string baseCurrencyCode,
            string counterCurrencyCode,
            DateTime dateFrom,
            DateTime dateTo,
            CancellationToken cancellationToken)
            => this.MapAsync(() => this.dataRepository.SearchAsync(
                foreignExchangeID,
                baseCurrencyCode,
                counterCurrencyCode,
                dateFrom,
                dateTo,
                cancellationToken));
    }
}
