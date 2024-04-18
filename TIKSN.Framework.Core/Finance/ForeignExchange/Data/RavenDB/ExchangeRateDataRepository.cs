using Raven.Client.Documents;
using TIKSN.Data.RavenDB;

namespace TIKSN.Finance.ForeignExchange.Data.RavenDB;

public class ExchangeRateDataRepository : RavenRepository<ExchangeRateDataEntity, Guid>, IExchangeRateDataRepository
{
    public ExchangeRateDataRepository(IRavenSessionProvider sessionProvider)
        : base(sessionProvider, "ExchangeRates")
    {
    }

    public async Task<IReadOnlyList<ExchangeRateDataEntity>> SearchAsync(
    Guid foreignExchangeID,
    string baseCurrencyCode,
    string counterCurrencyCode,
    DateTime dateFrom,
    DateTime dateTo,
    CancellationToken cancellationToken)
    => await this.SessionProvider.Session.Query<ExchangeRateDataEntity>(collectionName: this.CollectionName)
        .Where(item =>
            item.BaseCurrencyCode == baseCurrencyCode &&
            item.CounterCurrencyCode == counterCurrencyCode &&
            item.ForeignExchangeID == foreignExchangeID &&
            item.AsOn >= dateFrom && item.AsOn <= dateTo)
        .ToArrayAsync(cancellationToken).ConfigureAwait(false);

    public async Task<IReadOnlyList<ExchangeRateDataEntity>> SearchAsync(
        string baseCurrencyCode,
        string counterCurrencyCode,
        DateTime dateFrom,
        DateTime dateTo,
        CancellationToken cancellationToken)
        => await this.SessionProvider.Session.Query<ExchangeRateDataEntity>(collectionName: this.CollectionName)
            .Where(item =>
                item.BaseCurrencyCode == baseCurrencyCode &&
                item.CounterCurrencyCode == counterCurrencyCode &&
                item.AsOn >= dateFrom && item.AsOn <= dateTo)
            .ToArrayAsync(cancellationToken).ConfigureAwait(false);
}
