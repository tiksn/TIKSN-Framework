using System;
using TIKSN.Data.Mongo;

namespace TIKSN.Framework.IntegrationTests.Data.Mongo
{
    public interface ITestMongoRepository : IMongoRepository<TestMongoEntity, Guid>
    {
    }
}
