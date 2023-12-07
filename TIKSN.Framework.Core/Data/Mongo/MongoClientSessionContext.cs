using LanguageExt;
using MongoDB.Driver;

namespace TIKSN.Data.Mongo;

public class MongoClientSessionContext : IMongoClientSessionStore, IMongoClientSessionProvider
{
    private Option<IClientSessionHandle> clientSessionHandle = Option<IClientSessionHandle>.None;

    public void ClearClientSessionHandle() => this.clientSessionHandle = Option<IClientSessionHandle>.None;

    public Option<IClientSessionHandle> GetClientSessionHandle() => this.clientSessionHandle;

    public void SetClientSessionHandle(IClientSessionHandle clientSessionHandle)
    {
        ArgumentNullException.ThrowIfNull(clientSessionHandle);

        this.clientSessionHandle = Option<IClientSessionHandle>.Some(clientSessionHandle);
    }
}
