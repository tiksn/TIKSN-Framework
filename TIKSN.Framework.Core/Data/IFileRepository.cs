namespace TIKSN.Data;

public interface IFileRepository
{
    public Task DeleteAsync(string path, CancellationToken cancellationToken);

    public Task<IFile> DownloadAsync(string path, CancellationToken cancellationToken);

    public Task<bool> ExistsAsync(string path, CancellationToken cancellationToken);

    public Task UploadAsync(string path, byte[] content, CancellationToken cancellationToken);
}

public interface IFileRepository<TIdentity> where TIdentity : IEquatable<TIdentity>
{
    public Task DeleteByIdAsync(TIdentity id, CancellationToken cancellationToken);

    public Task<IFile<TIdentity>> DownloadByIdAsync(TIdentity id, CancellationToken cancellationToken);

    public Task<bool> ExistsByIdAsync(TIdentity id, CancellationToken cancellationToken);

    public Task UploadByIdAsync(TIdentity id, string path, byte[] content, CancellationToken cancellationToken);
}

public interface IFileRepository<TIdentity, TMetadata> where TIdentity : IEquatable<TIdentity>
{
    public Task<IFileInfo<TIdentity, TMetadata>> DownloadOnlyMetadataAsync(TIdentity id,
        CancellationToken cancellationToken);

    public Task<IFile<TIdentity, TMetadata>> DownloadWithMetadataAsync(TIdentity id, CancellationToken cancellationToken);

    public Task UploadAsync(TIdentity id, string path, byte[] content, TMetadata metadata,
        CancellationToken cancellationToken);
}
