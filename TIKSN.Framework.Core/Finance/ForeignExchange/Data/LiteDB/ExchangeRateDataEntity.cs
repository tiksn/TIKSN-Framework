using LiteDB;
using TIKSN.Data;

namespace TIKSN.Finance.ForeignExchange.Data.LiteDB;

public class ExchangeRateDataEntity : IEntity<Guid>
{
    public DateTime AsOn { get; set; }

    public string? BaseCurrencyCode { get; set; }

    public string? CounterCurrencyCode { get; set; }

    public virtual ForeignExchangeEntity? ForeignExchange { get; set; }

    public Guid? ForeignExchangeID { get; set; }

    [BsonId]
    public Guid ID { get; set; }

    public decimal Rate { get; set; }
}
