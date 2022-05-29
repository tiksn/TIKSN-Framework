using System;
using System.Collections.Generic;
using TIKSN.Data;

namespace TIKSN.Finance.ForeignExchange.Data
{
    public class ForeignExchangeEntity : IEntity<Guid>
    {
        public ForeignExchangeEntity() => this.ExchangeRates = new HashSet<ExchangeRateEntity>();
        public Guid ID { get; set; }
        public int LongNameKey { get; set; }
        public int ShortNameKey { get; set; }
        public string CountryCode { get; set; }
        public virtual ICollection<ExchangeRateEntity> ExchangeRates { get; set; }
    }
}
