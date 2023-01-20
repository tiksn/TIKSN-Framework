using System;
using TIKSN.Data;

namespace TIKSN.Finance.ForeignExchange.Data
{
    public class ExchangeRateEntity : IEntity<Guid>
    {
        public DateTime AsOn { get; set; }

        public string BaseCurrencyCode { get; set; }

        public string CounterCurrencyCode { get; set; }

        public Guid? ForeignExchangeID { get; set; }

        public Guid ID { get; set; }

        public decimal Rate { get; set; }
    }
}
