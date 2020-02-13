using LiteDB;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace TIKSN.Data.LiteDB
{
    public class LiteDbFileRepository<TIdentity, TMetadata> : IFileRepository<TIdentity>, IFileRepository<TIdentity, TMetadata> where TIdentity : IEquatable<TIdentity>
    {
        private readonly ILiteStorage<TIdentity> _liteStorage;

        public LiteDbFileRepository(ILiteDbDatabaseProvider databaseProvider)
        {
            _liteStorage = databaseProvider.GetDatabase().GetStorage<TIdentity>();
        }

        public Task DeleteByIdAsync(TIdentity id, CancellationToken cancellationToken)
        {
            return Task.FromResult(_liteStorage.Delete(id));
        }

        public async Task<IFile<TIdentity>> DownloadByIdAsync(TIdentity id, CancellationToken cancellationToken)
        {
            using (var stream = new MemoryStream())
            {
                var fileInfo = _liteStorage.Download(id, stream);

                return new File<TIdentity>(fileInfo.Id, fileInfo.Filename, stream.ToArray());
            }
        }

        public async Task<IFileInfo<TIdentity, TMetadata>> DownloadOnlyMetadataAsync(TIdentity id, CancellationToken cancellationToken)
        {
            var fileInfo = _liteStorage.FindById(id);

            return new FileInfo<TIdentity, TMetadata>(fileInfo.Id, fileInfo.Filename, BsonMapper.Global.ToObject<TMetadata>(fileInfo.Metadata));
        }

        public async Task<IFile<TIdentity, TMetadata>> DownloadWithMetadataAsync(TIdentity id, CancellationToken cancellationToken)
        {
            using (var stream = new MemoryStream())
            {
                var fileInfo = _liteStorage.Download(id, stream);

                return new File<TIdentity, TMetadata>(fileInfo.Id, fileInfo.Filename, BsonMapper.Global.ToObject<TMetadata>(fileInfo.Metadata), stream.ToArray());
            }
        }

        public Task<bool> ExistsByIdAsync(TIdentity id, CancellationToken cancellationToken)
        {
            return Task.FromResult(_liteStorage.Exists(id));
        }

        public async Task UploadAsync(TIdentity id, string path, byte[] content, TMetadata metadata, CancellationToken cancellationToken)
        {
            using (var stream = new MemoryStream(content))
            {
                _liteStorage.Upload(id, path, stream);
                _liteStorage.SetMetadata(id, BsonMapper.Global.ToDocument(metadata));
            }
        }

        public async Task UploadByIdAsync(TIdentity id, string path, byte[] content, CancellationToken cancellationToken)
        {
            using (var stream = new MemoryStream(content))
            {
                _liteStorage.Upload(id, path, stream);
            }
        }
    }
}