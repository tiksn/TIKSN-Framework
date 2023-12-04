using TIKSN.Data.Mongo;
using TIKSN.Mapping;

namespace TIKSN.Finance.ForeignExchange.Data.Mongo;

public class ExchangeRateRepositoryAdapter
    : MongoRepositoryAdapter<ExchangeRateEntity, Guid, ExchangeRateDataEntity, Guid>
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
            dataRepository) => this.dataRepository = dataRepository ?? throw new ArgumentNullException(nameof(dataRepository));

    public Task<IReadOnlyList<ExchangeRateEntity>> SearchAsync(
        string baseCurrencyCode,
        string counterCurrencyCode,
        DateTime dateFrom,
        DateTime dateTo,
        CancellationToken cancellationToken)
        => this.MapAsync(() => this.dataRepository.SearchAsync(
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
        => this.MapAsync(() => this.dataRepository.SearchAsync(
            foreignExchangeID,
            baseCurrencyCode,
            counterCurrencyCode,
            dateFrom,
            dateTo,
            cancellationToken));
}
