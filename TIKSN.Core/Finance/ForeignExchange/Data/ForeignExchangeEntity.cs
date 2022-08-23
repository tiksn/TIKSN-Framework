using System;
using System.Collections.Generic;
using TIKSN.Data;
using LiteBsonIdAttribute = LiteDB.BsonIdAttribute;
using MongoBsonIdAttribute = MongoDB.Bson.Serialization.Attributes.BsonIdAttribute;

namespace TIKSN.Finance.ForeignExchange.Data
{
    public class ForeignExchangeEntity : IEntity<Guid>
    {
        public ForeignExchangeEntity() => this.ExchangeRates = new HashSet<ExchangeRateEntity>();

        public string CountryCode { get; set; }

        public virtual ICollection<ExchangeRateEntity> ExchangeRates { get; set; }

        [MongoBsonId]
        [LiteBsonId]
        public Guid ID { get; set; }

        public int LongNameKey { get; set; }
        public int ShortNameKey { get; set; }
    }
}
