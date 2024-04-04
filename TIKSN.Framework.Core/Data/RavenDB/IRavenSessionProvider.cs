using Raven.Client.Documents.Session;

namespace TIKSN.Data.RavenDB;

public interface IRavenSessionProvider
{
    IAsyncDocumentSession Session { get; }
}
