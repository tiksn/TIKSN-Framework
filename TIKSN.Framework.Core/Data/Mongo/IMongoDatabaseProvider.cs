using MongoDB.Driver;

namespace TIKSN.Data.Mongo;

public interface IMongoDatabaseProvider
{
    IMongoDatabase GetDatabase();
}
