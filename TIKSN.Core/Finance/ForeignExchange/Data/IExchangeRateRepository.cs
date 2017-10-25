using TIKSN.Data;

namespace TIKSN.Finance.ForeignExchange.Data
{
    public interface IExchangeRateRepository : IQueryRepository<ExchangeRateEntity, int>
    {
    }
}
