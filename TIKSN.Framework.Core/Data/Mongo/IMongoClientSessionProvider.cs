using LanguageExt;
using MongoDB.Driver;

namespace TIKSN.Data.Mongo;

public interface IMongoClientSessionProvider
{
    Option<IClientSessionHandle> GetClientSessionHandle();
}
