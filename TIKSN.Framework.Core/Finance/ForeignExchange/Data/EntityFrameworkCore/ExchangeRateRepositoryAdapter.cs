using TIKSN.Data;
using TIKSN.Mapping;

namespace TIKSN.Finance.ForeignExchange.Data.EntityFrameworkCore;

public class ExchangeRateRepositoryAdapter
    : RepositoryAdapter<ExchangeRateEntity, Guid, ExchangeRateDataEntity, Guid>
    , IExchangeRateRepository
{
    public ExchangeRateRepositoryAdapter(
        IExchangeRateDataRepository dataRepository,
        IMapper<ExchangeRateEntity, ExchangeRateDataEntity> domainEntityToDataEntityMapper,
        IMapper<ExchangeRateDataEntity, ExchangeRateEntity> dataEntityToDomainEntityMapper,
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

    protected IExchangeRateDataRepository DataRepository { get; }

    public Task<IReadOnlyList<ExchangeRateEntity>> SearchAsync(
        string baseCurrencyCode,
        string counterCurrencyCode,
        DateTime dateFrom,
        DateTime dateTo,
        CancellationToken cancellationToken)
        => this.MapAsync(() => this.DataRepository.SearchAsync(
            baseCurrencyCode,
            counterCurrencyCode,
            dateFrom,
            dateTo,
            cancellationToken));

    public Task<IReadOnlyList<ExchangeRateEntity>> SearchAsync(
        Guid foreignExchangeID,
        string baseCurrencyCode,
        string counterCurrencyCode,
        DateTime dateFrom,
        DateTime dateTo,
        CancellationToken cancellationToken)
        => this.MapAsync(() => this.DataRepository.SearchAsync(
            foreignExchangeID,
            baseCurrencyCode,
            counterCurrencyCode,
            dateFrom,
            dateTo,
            cancellationToken));
}
