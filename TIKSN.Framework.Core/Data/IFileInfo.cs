namespace TIKSN.Data;

public interface IFileInfo
{
    public string Path { get; }
}

public interface IFileInfo<TIdentity> : IFileInfo where TIdentity : IEquatable<TIdentity>
{
    public TIdentity ID { get; }
}

public interface IFileInfo<TIdentity, TMetadata> : IFileInfo<TIdentity> where TIdentity : IEquatable<TIdentity>
{
    public TMetadata Metadata { get; }
}
