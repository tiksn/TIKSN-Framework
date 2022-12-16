using System;
using TIKSN.Data;
using TIKSN.Data.LiteDB;

namespace TIKSN.Finance.ForeignExchange.Data.LiteDB
{
    public class ForeignExchangeRepositoryAdapter
        : LiteDbRepositoryAdapter<ForeignExchangeEntity, Guid, ForeignExchangeDataEntity, Guid>
        , IForeignExchangeRepository
    {
        protected readonly IForeignExchangeDataRepository dataRepository;

        public ForeignExchangeRepositoryAdapter(
            IForeignExchangeDataRepository dataRepository,
            IMapper<ForeignExchangeEntity, ForeignExchangeDataEntity> domainEntityToDataEntityMapper,
            IMapper<ForeignExchangeDataEntity, ForeignExchangeEntity> dataEntityToDomainEntityMapper) : base(
                domainEntityToDataEntityMapper,
                dataEntityToDomainEntityMapper,
                IdentityMapper<Guid>.Instance,
                IdentityMapper<Guid>.Instance,
                dataRepository) => this.dataRepository = dataRepository ?? throw new ArgumentNullException(nameof(dataRepository));
    }
}
