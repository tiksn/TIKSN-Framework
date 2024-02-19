using TIKSN.Data;
using TIKSN.Mapping;

namespace TIKSN.Finance.ForeignExchange.Data.EntityFrameworkCore;

public class ForeignExchangeRepositoryAdapter
    : RepositoryAdapter<ForeignExchangeEntity, Guid, ForeignExchangeDataEntity, Guid>
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
            dataRepository,
            dataRepository,
            dataRepository)
            => this.DataRepository = dataRepository ?? throw new ArgumentNullException(nameof(dataRepository));

    protected IForeignExchangeDataRepository DataRepository { get; }
}
