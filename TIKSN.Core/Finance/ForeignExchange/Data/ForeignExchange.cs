using System.Collections.Generic;

namespace TIKSN.Finance.ForeignExchange.Data
{
    public class ForeignExchange
    {
        public ForeignExchange()
        {
            ExchangeRates = new HashSet<ExchangeRate>();
        }

        public long Id { get; set; }
        public long LongNameKey { get; set; }
        public long ShortNameKey { get; set; }
        public string CountryCode { get; set; }

        public virtual ICollection<ExchangeRate> ExchangeRates { get; set; }
    }
}
