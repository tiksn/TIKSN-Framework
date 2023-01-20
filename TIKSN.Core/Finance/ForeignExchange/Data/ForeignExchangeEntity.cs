using System;
using TIKSN.Data;

namespace TIKSN.Finance.ForeignExchange.Data
{
    public class ForeignExchangeEntity : IEntity<Guid>
    {
        public string CountryCode { get; set; }

        public Guid ID { get; set; }

        public int LongNameKey { get; set; }
        public int ShortNameKey { get; set; }
    }
}
