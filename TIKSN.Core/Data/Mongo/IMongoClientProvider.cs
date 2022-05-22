using MongoDB.Driver;

namespace TIKSN.Data.Mongo
{
    public interface IMongoClientProvider
    {
        IMongoClient GetMongoClient();
    }
}
