using LanguageExt;
using MongoDB.Driver;

namespace TIKSN.Data.Mongo;

public interface IMongoClientSessionProvider
{
    public Option<IClientSessionHandle> GetClientSessionHandle();
}
