using System;
using MongoDB.Bson.Serialization.Attributes;
using TIKSN.Data;

namespace TIKSN.IntegrationTests.Data.Mongo;

public class TestMongoEntity : IEntity<Guid>
{
    [BsonId]
    public Guid ID { get; set; }

    public Guid Value { get; set; }

    public int Version { get; set; }
}
