using System;
using MongoDB.Bson.Serialization.Attributes;
using TIKSN.Data;

namespace TIKSN.IntegrationTests.Data.Mongo;

public class TestMongoEntity : IEntity<Guid>
{
    public Guid Value { get; set; }

    public int Version { get; set; }

    [BsonId] public Guid ID { get; set; }
}
