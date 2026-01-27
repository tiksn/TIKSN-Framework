using MongoDB.Driver;

namespace TIKSN.Data.Mongo;

public interface IMongoDatabaseProvider
{
    public IMongoDatabase GetDatabase();
}
