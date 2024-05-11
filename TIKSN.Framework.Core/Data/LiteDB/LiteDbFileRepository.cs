using LiteDB;
using Microsoft.IO;

namespace TIKSN.Data.LiteDB;

public class LiteDbFileRepository<TIdentity, TMetadata> : IFileRepository<TIdentity>,
    IFileRepository<TIdentity, TMetadata> where TIdentity : IEquatable<TIdentity>
{
    private readonly ILiteStorage<TIdentity> liteStorage;
    private readonly RecyclableMemoryStreamManager recyclableMemoryStreamManager;

    public LiteDbFileRepository(
        ILiteDbDatabaseProvider databaseProvider,
        RecyclableMemoryStreamManager recyclableMemoryStreamManager)
    {
        ArgumentNullException.ThrowIfNull(databaseProvider);

        this.recyclableMemoryStreamManager = recyclableMemoryStreamManager
            ?? throw new ArgumentNullException(nameof(recyclableMemoryStreamManager));
        this.liteStorage = databaseProvider.GetDatabase().GetStorage<TIdentity>();
    }

    public Task DeleteByIdAsync(TIdentity id, CancellationToken cancellationToken)
    {
        _ = this.liteStorage.Delete(id);

        return Task.CompletedTask;
    }

    public Task<IFile<TIdentity>> DownloadByIdAsync(
        TIdentity id,
        CancellationToken cancellationToken)
    {
#pragma warning disable RCS1261 // Resource can be disposed asynchronously
        using var stream = this.recyclableMemoryStreamManager.GetStream();
#pragma warning restore RCS1261 // Resource can be disposed asynchronously
        var fileInfo = this.liteStorage.Download(id, stream);

        var file = new File<TIdentity>(fileInfo.Id, fileInfo.Filename, stream.ToArray());
        return Task.FromResult<IFile<TIdentity>>(file);
    }

    public Task<IFileInfo<TIdentity, TMetadata>> DownloadOnlyMetadataAsync(
        TIdentity id,
        CancellationToken cancellationToken)
    {
        var fileInfo = this.liteStorage.FindById(id);

        var info = new FileInfo<TIdentity, TMetadata>(fileInfo.Id, fileInfo.Filename,
            BsonMapper.Global.ToObject<TMetadata>(fileInfo.Metadata));
        return Task.FromResult<IFileInfo<TIdentity, TMetadata>>(info);
    }

    public Task<IFile<TIdentity, TMetadata>> DownloadWithMetadataAsync(
        TIdentity id,
        CancellationToken cancellationToken)
    {
#pragma warning disable RCS1261 // Resource can be disposed asynchronously
        using var stream = this.recyclableMemoryStreamManager.GetStream();
#pragma warning restore RCS1261 // Resource can be disposed asynchronously
        var fileInfo = this.liteStorage.Download(id, stream);

        return Task.FromResult<IFile<TIdentity, TMetadata>>(
            new File<TIdentity, TMetadata>(
                fileInfo.Id,
                fileInfo.Filename,
                BsonMapper.Global.ToObject<TMetadata>(fileInfo.Metadata),
                stream.ToArray()));
    }

    public Task<bool> ExistsByIdAsync(TIdentity id, CancellationToken cancellationToken) =>
        Task.FromResult(this.liteStorage.Exists(id));

    public Task UploadAsync(
        TIdentity id,
        string path,
        byte[] content,
        TMetadata metadata,
        CancellationToken cancellationToken)
    {
#pragma warning disable RCS1261 // Resource can be disposed asynchronously
        using var stream = new MemoryStream(content);
#pragma warning restore RCS1261 // Resource can be disposed asynchronously
        _ = this.liteStorage.Upload(id, path, stream);
        _ = this.liteStorage.SetMetadata(id, BsonMapper.Global.ToDocument(metadata));

        return Task.CompletedTask;
    }

    public Task UploadByIdAsync(
        TIdentity id,
        string path,
        byte[] content,
        CancellationToken cancellationToken)
    {
#pragma warning disable RCS1261 // Resource can be disposed asynchronously
        using var stream = new MemoryStream(content);
#pragma warning restore RCS1261 // Resource can be disposed asynchronously
        _ = this.liteStorage.Upload(id, path, stream);

        return Task.CompletedTask;
    }
}
