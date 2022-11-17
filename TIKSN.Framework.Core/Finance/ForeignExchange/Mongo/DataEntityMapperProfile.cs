using Microsoft.Extensions.DependencyInjection;
using TIKSN.Data;
using TIKSN.Finance.ForeignExchange.Data;
using TIKSN.Finance.ForeignExchange.Data.Mongo;

namespace TIKSN.Finance.ForeignExchange.Mongo
{
    public class DataEntityMapperProfile : MapperProfile
    {
        public DataEntityMapperProfile(IServiceCollection services) : base(services)
        {
            AddMapper<ExchangeRateEntity, ExchangeRateDataEntity>();
            AddMapper<ExchangeRateDataEntity, ExchangeRateEntity>();

            AddMapper<ForeignExchangeEntity, ForeignExchangeDataEntity>();
            AddMapper<ForeignExchangeDataEntity, ForeignExchangeEntity>();
        }
    }
}
