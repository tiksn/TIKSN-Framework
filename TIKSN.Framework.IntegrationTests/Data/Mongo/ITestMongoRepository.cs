using System;

namespace TIKSN.Data.Mongo.IntegrationTests;

public interface ITestMongoRepository : IMongoRepository<TestMongoEntity, Guid>
{
}
