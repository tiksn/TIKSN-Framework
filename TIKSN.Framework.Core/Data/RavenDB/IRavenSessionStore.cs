using Raven.Client.Documents.Session;

namespace TIKSN.Data.RavenDB;

public interface IRavenSessionStore
{
    void SetSession(IAsyncDocumentSession session);
}
