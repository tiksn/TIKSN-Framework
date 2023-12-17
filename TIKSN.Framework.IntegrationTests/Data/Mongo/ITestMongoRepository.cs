using System;
using TIKSN.Data.Mongo;

namespace TIKSN.IntegrationTests.Data.Mongo;

public interface ITestMongoRepository : IMongoRepository<TestMongoEntity, Guid>
{
}
