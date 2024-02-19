namespace TIKSN.Data;

public interface IFile : IFileInfo
{
    IReadOnlyList<byte> Content { get; }
}

public interface IFile<TIdentity> : IFile, IFileInfo<TIdentity>
    where TIdentity : IEquatable<TIdentity>;

public interface IFile<TIdentity, TMetadata> : IFile, IFileInfo<TIdentity, TMetadata>
    where TIdentity : IEquatable<TIdentity>;
