using LiteDB;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace TIKSN.Data.LiteDB
{
    public class LiteDbFileRepository<TMetadata> : IFileRepository<string>, IFileRepository<string, TMetadata>
    {
        private readonly LiteStorage _liteStorage;

        public LiteDbFileRepository(ILiteDbDatabaseProvider databaseProvider)
        {
            _liteStorage = databaseProvider.GetDatabase().FileStorage;
        }

        public Task DeleteByIdAsync(string id, CancellationToken cancellationToken)
        {
            return Task.FromResult(_liteStorage.Delete(id));
        }

        public async Task<IFile<string>> DownloadByIdAsync(string id, CancellationToken cancellationToken)
        {
            using (var stream = new MemoryStream())
            {
                var fileInfo = _liteStorage.Download(id, stream);

                return new File<string>(fileInfo.Id, fileInfo.Filename, stream.ToArray());
            }
        }

        public async Task<IFileInfo<string, TMetadata>> DownloadOnlyMetadataAsync(string id, CancellationToken cancellationToken)
        {
            var fileInfo = _liteStorage.FindById(id);

            return new FileInfo<string, TMetadata>(fileInfo.Id, fileInfo.Filename, BsonMapper.Global.ToObject<TMetadata>(fileInfo.Metadata));
        }

        public async Task<IFile<string, TMetadata>> DownloadWithMetadataAsync(string id, CancellationToken cancellationToken)
        {
            using (var stream = new MemoryStream())
            {
                var fileInfo = _liteStorage.Download(id, stream);

                return new File<string, TMetadata>(fileInfo.Id, fileInfo.Filename, BsonMapper.Global.ToObject<TMetadata>(fileInfo.Metadata), stream.ToArray());
            }
        }

        public Task<bool> ExistsByIdAsync(string id, CancellationToken cancellationToken)
        {
            return Task.FromResult(_liteStorage.Exists(id));
        }

        public async Task UploadAsync(string id, string path, byte[] content, TMetadata metadata, CancellationToken cancellationToken)
        {
            using (var stream = new MemoryStream(content))
            {
                _liteStorage.Upload(id, path, stream);
                _liteStorage.SetMetadata(id, BsonMapper.Global.ToDocument(metadata));
            }
        }

        public async Task UploadByIdAsync(string id, string path, byte[] content, CancellationToken cancellationToken)
        {
            using (var stream = new MemoryStream(content))
            {
                _liteStorage.Upload(id, path, stream);
            }
        }
    }
}