using MongoDB.Driver;

namespace TIKSN.Data.Mongo;

public interface IMongoClientSessionStore
{
    public void SetClientSessionHandle(IClientSessionHandle clientSessionHandle);
    public void ClearClientSessionHandle();
}
