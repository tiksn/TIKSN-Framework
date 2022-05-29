using TIKSN.Data;

namespace TIKSN.Finance.ForeignExchange.ExchangeRateService.IntegrationTests
{
    public class TestUnitOfWorkFactory : IUnitOfWorkFactory
    {
        public IUnitOfWork Create() => new TestUnitOfWork();
    }
}
