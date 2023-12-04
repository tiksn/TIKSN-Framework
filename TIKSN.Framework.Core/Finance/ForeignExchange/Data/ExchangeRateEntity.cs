using TIKSN.Data;

namespace TIKSN.Finance.ForeignExchange.Data;

public class ExchangeRateEntity : IEntity<Guid>
{
    public ExchangeRateEntity(
        Guid id,
        string baseCurrencyCode,
        string counterCurrencyCode,
        Guid? foreignExchangeId,
        DateTime asOn,
        decimal rate)
    {
        this.ID = id;
        this.BaseCurrencyCode = baseCurrencyCode;
        this.CounterCurrencyCode = counterCurrencyCode;
        this.ForeignExchangeID = foreignExchangeId;
        this.AsOn = asOn;
        this.Rate = rate;
    }

    public DateTime AsOn { get; }

    public string BaseCurrencyCode { get; }

    public string CounterCurrencyCode { get; }

    public Guid? ForeignExchangeID { get; }

    public Guid ID { get; }

    public decimal Rate { get; }
}
