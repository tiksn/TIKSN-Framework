namespace TIKSN.Data;

public interface IFileInfo
{
    string Path { get; }
}

public interface IFileInfo<TIdentity> : IFileInfo where TIdentity : IEquatable<TIdentity>
{
    TIdentity ID { get; }
}

public interface IFileInfo<TIdentity, TMetadata> : IFileInfo<TIdentity> where TIdentity : IEquatable<TIdentity>
{
    TMetadata Metadata { get; }
}
