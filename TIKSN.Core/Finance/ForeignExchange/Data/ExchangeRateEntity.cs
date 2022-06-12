using System;
using TIKSN.Data;
using LiteBsonIdAttribute = LiteDB.BsonIdAttribute;
using MongoBsonIdAttribute = MongoDB.Bson.Serialization.Attributes.BsonIdAttribute;

namespace TIKSN.Finance.ForeignExchange.Data
{
    public class ExchangeRateEntity : IEntity<Guid>
    {
        public DateTime AsOn { get; set; }

        public string BaseCurrencyCode { get; set; }

        public string CounterCurrencyCode { get; set; }

        public virtual ForeignExchangeEntity ForeignExchange { get; set; }

        public Guid? ForeignExchangeID { get; set; }

        [MongoBsonId]
        [LiteBsonId]
        public Guid ID { get; set; }

        public decimal Rate { get; set; }
    }
}
