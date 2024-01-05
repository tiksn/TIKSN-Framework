using LiteDB;
using TIKSN.Data;

namespace TIKSN.Finance.ForeignExchange.Data.LiteDB;

public class ForeignExchangeDataEntity : IEntity<Guid>
{
    public string CountryCode { get; set; }

    [BsonId]
    public Guid ID { get; set; }
}
