namespace TIKSN.Data;

public interface IFileRepository
{
    Task DeleteAsync(string path, CancellationToken cancellationToken);

    Task<IFile> DownloadAsync(string path, CancellationToken cancellationToken);

    Task<bool> ExistsAsync(string path, CancellationToken cancellationToken);

    Task UploadAsync(string path, byte[] content, CancellationToken cancellationToken);
}

public interface IFileRepository<TIdentity> where TIdentity : IEquatable<TIdentity>
{
    Task DeleteByIdAsync(TIdentity id, CancellationToken cancellationToken);

    Task<IFile<TIdentity>> DownloadByIdAsync(TIdentity id, CancellationToken cancellationToken);

    Task<bool> ExistsByIdAsync(TIdentity id, CancellationToken cancellationToken);

    Task UploadByIdAsync(TIdentity id, string path, byte[] content, CancellationToken cancellationToken);
}

public interface IFileRepository<TIdentity, TMetadata> where TIdentity : IEquatable<TIdentity>
{
    Task<IFileInfo<TIdentity, TMetadata>> DownloadOnlyMetadataAsync(TIdentity id,
        CancellationToken cancellationToken);

    Task<IFile<TIdentity, TMetadata>> DownloadWithMetadataAsync(TIdentity id, CancellationToken cancellationToken);

    Task UploadAsync(TIdentity id, string path, byte[] content, TMetadata metadata,
        CancellationToken cancellationToken);
}
