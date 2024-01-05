using TIKSN.Data;

namespace TIKSN.Finance.ForeignExchange.Data.EntityFrameworkCore;

public class ForeignExchangeDataEntity : IEntity<Guid>
{
    public ForeignExchangeDataEntity() => this.ExchangeRates = new HashSet<ExchangeRateDataEntity>();

    public string CountryCode { get; set; }

    public virtual ICollection<ExchangeRateDataEntity> ExchangeRates { get; set; }

    public Guid ID { get; set; }
}
