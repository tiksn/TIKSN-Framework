using TIKSN.Data;

namespace TIKSN.Finance.ForeignExchange.Data;

public class ForeignExchangeEntity : IEntity<Guid>
{
    public ForeignExchangeEntity(Guid id, string countryCode)
    {
        this.ID = id;
        this.CountryCode = countryCode;
    }

    public string CountryCode { get; }

    public Guid ID { get; }
}
