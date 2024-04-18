using TIKSN.Data;

namespace TIKSN.Finance.ForeignExchange.Data.RavenDB;

public class ForeignExchangeDataEntity : IEntity<Guid>
{
    public string CountryCode { get; set; }

    public Guid ID { get; set; }
}
