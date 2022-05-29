using System;
using MongoDB.Bson.Serialization.Attributes;

namespace TIKSN.Data.Mongo.IntegrationTests
{
    public class TestMongoEntity : IEntity<Guid>
    {
        public Guid Value { get; set; }

        public int Version { get; set; }

        [BsonId] public Guid ID { get; set; }
    }
}
