using MongoDB.Bson.Serialization.Attributes;
using TIKSN.Data;

namespace TIKSN.Finance.ForeignExchange.Data.Mongo;

public class ForeignExchangeDataEntity : IEntity<Guid>
{
    public string CountryCode { get; set; }

    [BsonId]
    public Guid ID { get; set; }

    public int LongNameKey { get; set; }
    public int ShortNameKey { get; set; }
}
