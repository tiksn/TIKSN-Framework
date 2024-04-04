namespace TIKSN.Data.RavenDB;

public interface IRavenRepository<TDocument, TIdentity> :
    IRepository<TDocument>,
    IQueryRepository<TDocument, TIdentity>,
    IStreamRepository<TDocument> where TDocument : IEntity<TIdentity> where TIdentity : IEquatable<TIdentity>;
