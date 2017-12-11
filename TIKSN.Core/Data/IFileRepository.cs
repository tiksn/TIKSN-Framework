using System;
using System.Threading;
using System.Threading.Tasks;

namespace TIKSN.Data
{
    public interface IFileRepository
    {
        Task DeleteAsync(string path, CancellationToken cancellationToken = default);

        Task<IFile> DownloadAsync(string path, CancellationToken cancellationToken = default);

        Task<bool> ExistsAsync(string path, CancellationToken cancellationToken = default);

        Task UploadAsync(string path, byte[] content, CancellationToken cancellationToken = default);
    }

    public interface IFileRepository<TIdentity> where TIdentity : IEquatable<TIdentity>
    {
        Task DeleteByIdAsync(TIdentity id, CancellationToken cancellationToken = default);

        Task<bool> ExistsByIdAsync(TIdentity id, CancellationToken cancellationToken = default);

        Task<IFile<TIdentity>> DownloadByIdAsync(TIdentity id, CancellationToken cancellationToken = default);

        Task UploadByIdAsync(TIdentity id, string path, byte[] content, CancellationToken cancellationToken = default);
    }

    public interface IFileRepository<TIdentity, TMetadata> where TIdentity : IEquatable<TIdentity>
    {
        Task<IFile<TIdentity, TMetadata>> DownloadWithMetadataAsync(TIdentity id, CancellationToken cancellationToken = default);

        Task<IFileInfo<TIdentity, TMetadata>> DownloadOnlyMetadataAsync(TIdentity id, CancellationToken cancellationToken = default);

        Task UploadAsync(TIdentity id, string path, byte[] content, TMetadata metadata, CancellationToken cancellationToken = default);
    }
}