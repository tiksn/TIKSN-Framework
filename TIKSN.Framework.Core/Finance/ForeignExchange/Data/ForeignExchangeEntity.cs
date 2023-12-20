using TIKSN.Data;

namespace TIKSN.Finance.ForeignExchange.Data;

public class ForeignExchangeEntity : IEntity<Guid>
{
    public ForeignExchangeEntity(Guid id, string countryCode, int longNameKey, int shortNameKey)
    {
        this.ID = id;
        this.CountryCode = countryCode;
        this.LongNameKey = longNameKey;
        this.ShortNameKey = shortNameKey;
    }

    public string CountryCode { get; }

    public Guid ID { get; }

    public int LongNameKey { get; }
    public int ShortNameKey { get; }
}
