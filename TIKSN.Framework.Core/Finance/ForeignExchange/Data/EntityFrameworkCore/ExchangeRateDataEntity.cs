using TIKSN.Data;

namespace TIKSN.Finance.ForeignExchange.Data.EntityFrameworkCore;

public class ExchangeRateDataEntity : IEntity<Guid>
{
    public DateTime AsOn { get; set; }

    public string? BaseCurrencyCode { get; set; }

    public string? CounterCurrencyCode { get; set; }

    public virtual ForeignExchangeDataEntity? ForeignExchange { get; set; }

    public Guid? ForeignExchangeID { get; set; }

    public Guid ID { get; set; }

    public decimal Rate { get; set; }
}
