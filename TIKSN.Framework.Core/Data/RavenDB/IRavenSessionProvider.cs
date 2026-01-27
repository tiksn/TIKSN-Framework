using Raven.Client.Documents.Session;

namespace TIKSN.Data.RavenDB;

public interface IRavenSessionProvider
{
    public IAsyncDocumentSession Session { get; }
}
