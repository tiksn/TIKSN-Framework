using MongoDB.Driver;

namespace TIKSN.Data.Mongo;

public interface IMongoClientSessionStore
{
    void SetClientSessionHandle(IClientSessionHandle clientSessionHandle);
    void ClearClientSessionHandle();
}
