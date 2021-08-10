using System;
using TIKSN.Data;

namespace TIKSN.Finance.ForeignExchange.Data
{
    public class ExchangeRateEntity : IEntity<int>
    {
        public string BaseCurrencyCode { get; set; }
        public string CounterCurrencyCode { get; set; }
        public DateTimeOffset AsOn { get; set; }
        public decimal Rate { get; set; }
        public int? ForeignExchangeID { get; set; }

        public virtual ForeignExchangeEntity ForeignExchange { get; set; }
        public int ID { get; set; }
    }
}
