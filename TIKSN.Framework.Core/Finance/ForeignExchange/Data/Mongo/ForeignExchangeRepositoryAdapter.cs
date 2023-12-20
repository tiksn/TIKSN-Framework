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
        IMapper<ForeignExchangeDataEntity, ForeignExchangeEntity> dataEntityToDomainEntityMapper) : base(
            domainEntityToDataEntityMapper,
            dataEntityToDomainEntityMapper,
            IdentityMapper<Guid>.Instance,
            IdentityMapper<Guid>.Instance,
            dataRepository) => this.DataRepository = dataRepository ?? throw new ArgumentNullException(nameof(dataRepository));

    protected IForeignExchangeDataRepository DataRepository { get; }
}
