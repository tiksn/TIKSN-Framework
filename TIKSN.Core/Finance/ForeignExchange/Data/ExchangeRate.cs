using System;
using System.Collections.Generic;

namespace TIKSN.Finance.ForeignExchange.Data
{
    public class ExchangeRate
    {
        public long Id { get; set; }
        public string BaseCurrencyCode { get; set; }
        public string CounterCurrencyCode { get; set; }
        public string AsOn { get; set; }
        public string Rate { get; set; }
        public long? ForeignExchangeId { get; set; }

        public virtual ForeignExchange ForeignExchange { get; set; }
    }
}
