using LanguageExt;
using Raven.Client.Documents.Session;
using static LanguageExt.Prelude;

namespace TIKSN.Data.RavenDB;

public class RavenSessionContext : IRavenSessionStore, IRavenSessionProvider
{
    private Option<IAsyncDocumentSession> session;

    public IAsyncDocumentSession Session => this.session.Match(s => s, () => throw new InvalidOperationException("Raven Session is not initialized"));

    public void SetSession(IAsyncDocumentSession session) => this.session = Some(session);
}
