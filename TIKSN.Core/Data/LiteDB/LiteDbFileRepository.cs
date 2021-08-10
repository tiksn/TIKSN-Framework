using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using LiteDB;
using Microsoft.IO;

namespace TIKSN.Data.LiteDB
{
    public class LiteDbFileRepository<TIdentity, TMetadata> : IFileRepository<TIdentity>,
        IFileRepository<TIdentity, TMetadata> where TIdentity : IEquatable<TIdentity>
    {
        private readonly ILiteStorage<TIdentity> _liteStorage;
        private readonly RecyclableMemoryStreamManager _recyclableMemoryStreamManager;

        public LiteDbFileRepository(ILiteDbDatabaseProvider databaseProvider,
            RecyclableMemoryStreamManager recyclableMemoryStreamManager)
        {
            this._recyclableMemoryStreamManager = recyclableMemoryStreamManager ??
                                                  throw new ArgumentNullException(
                                                      nameof(recyclableMemoryStreamManager));
            this._liteStorage = databaseProvider.GetDatabase().GetStorage<TIdentity>();
        }

        public Task<IFileInfo<TIdentity, TMetadata>> DownloadOnlyMetadataAsync(TIdentity id,
            CancellationToken cancellationToken)
        {
            var fileInfo = this._liteStorage.FindById(id);

            var info = new FileInfo<TIdentity, TMetadata>(fileInfo.Id, fileInfo.Filename,
                BsonMapper.Global.ToObject<TMetadata>(fileInfo.Metadata));
            return Task.FromResult<IFileInfo<TIdentity, TMetadata>>(info);
        }

        public Task<IFile<TIdentity, TMetadata>> DownloadWithMetadataAsync(TIdentity id,
            CancellationToken cancellationToken)
        {
            using var stream = new RecyclableMemoryStream(this._recyclableMemoryStreamManager);
            var fileInfo = this._liteStorage.Download(id, stream);

            var file = new File<TIdentity, TMetadata>(fileInfo.Id, fileInfo.Filename,
                BsonMapper.Global.ToObject<TMetadata>(fileInfo.Metadata), stream.ToArray());
            return Task.FromResult<IFile<TIdentity, TMetadata>>(file);
        }

        public Task UploadAsync(TIdentity id, string path, byte[] content, TMetadata metadata,
            CancellationToken cancellationToken)
        {
            using var stream = new MemoryStream(content);
            _ = this._liteStorage.Upload(id, path, stream);
            _ = this._liteStorage.SetMetadata(id, BsonMapper.Global.ToDocument(metadata));
            return Task.CompletedTask;
        }

        public Task DeleteByIdAsync(TIdentity id, CancellationToken cancellationToken)
        {
            _ = this._liteStorage.Delete(id);

            return Task.CompletedTask;
        }

        public Task<IFile<TIdentity>> DownloadByIdAsync(TIdentity id, CancellationToken cancellationToken)
        {
            using var stream = new RecyclableMemoryStream(this._recyclableMemoryStreamManager);
            var fileInfo = this._liteStorage.Download(id, stream);

            var file = new File<TIdentity>(fileInfo.Id, fileInfo.Filename, stream.ToArray());
            return Task.FromResult<IFile<TIdentity>>(file);
        }

        public Task<bool> ExistsByIdAsync(TIdentity id, CancellationToken cancellationToken) =>
            Task.FromResult(this._liteStorage.Exists(id));

        public Task UploadByIdAsync(TIdentity id, string path, byte[] content,
            CancellationToken cancellationToken)
        {
            using var stream = new MemoryStream(content);
            _ = this._liteStorage.Upload(id, path, stream);
            return Task.CompletedTask;
        }
    }
}
