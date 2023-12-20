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

        this.recyclableMemoryStreamManager = recyclableMemoryStreamManager ??
                                              throw new ArgumentNullException(
                                                  nameof(recyclableMemoryStreamManager));
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
        using var stream = new RecyclableMemoryStream(this.recyclableMemoryStreamManager);
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

    public async Task<IFile<TIdentity, TMetadata>> DownloadWithMetadataAsync(
        TIdentity id,
        CancellationToken cancellationToken)
    {
        await using var stream = new RecyclableMemoryStream(this.recyclableMemoryStreamManager);
        var fileInfo = this.liteStorage.Download(id, stream);

        return new File<TIdentity, TMetadata>(fileInfo.Id, fileInfo.Filename,
            BsonMapper.Global.ToObject<TMetadata>(fileInfo.Metadata), stream.ToArray());
    }

    public Task<bool> ExistsByIdAsync(TIdentity id, CancellationToken cancellationToken) =>
        Task.FromResult(this.liteStorage.Exists(id));

    public async Task UploadAsync(
        TIdentity id,
        string path,
        byte[] content,
        TMetadata metadata,
        CancellationToken cancellationToken)
    {
        await using var stream = new MemoryStream(content);
        _ = this.liteStorage.Upload(id, path, stream);
        _ = this.liteStorage.SetMetadata(id, BsonMapper.Global.ToDocument(metadata));
    }

    public Task UploadByIdAsync(
        TIdentity id,
        string path,
        byte[] content,
        CancellationToken cancellationToken)
    {
        using var stream = new MemoryStream(content);
        _ = this.liteStorage.Upload(id, path, stream);
        return Task.CompletedTask;
    }
}
