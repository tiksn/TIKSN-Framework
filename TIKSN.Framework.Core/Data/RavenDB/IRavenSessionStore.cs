using Raven.Client.Documents.Session;

namespace TIKSN.Data.RavenDB;

public interface IRavenSessionStore
{
    public void SetSession(IAsyncDocumentSession session);
}
