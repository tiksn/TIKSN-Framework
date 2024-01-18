using TIKSN.Data.Mongo;
using TIKSN.Mapping;

namespace TIKSN.Finance.ForeignExchange.Data.Mongo;

public class ForeignExchangeRepositoryAdapter
    : MongoRepositoryAdapter<ForeignExchangeEntity, Guid, ForeignExchangeDataEntity, Guid>
    , IForeignExchangeRepository
{
    public ForeignExchangeRepositoryAdapter(
        IForeignExchangeDataRepository dataRepository,
        IMapper<ForeignExchangeEntity, ForeignExchangeDataEntity> domainEntityToDataEntityMapper,
        IMapper<ForeignExchangeDataEntity, ForeignExchangeEntity> dataEntityToDomainEntityMapper,
        IMapper<Guid, Guid> domainIdentityToDataIdentityMapper,
        IMapper<Guid, Guid> dataIdentityToDomainIdentityMapper) : base(
            domainEntityToDataEntityMapper,
            dataEntityToDomainEntityMapper,
            domainIdentityToDataIdentityMapper,
            dataIdentityToDomainIdentityMapper,
            dataRepository) => this.DataRepository = dataRepository ?? throw new ArgumentNullException(nameof(dataRepository));

    protected IForeignExchangeDataRepository DataRepository { get; }
}
