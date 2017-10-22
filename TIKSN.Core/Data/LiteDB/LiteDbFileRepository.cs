using LiteDB;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace TIKSN.Data.LiteDB
{
    public class LiteDbFileRepository : IFileRepository
    {
        private LiteStorage _liteStorage;

        public LiteDbFileRepository(ILiteDbDatabaseProvider databaseProvider)
        {
            _liteStorage = databaseProvider.GetDatabase().FileStorage;
        }

        public Task DeleteAsync(string path, CancellationToken cancellationToken = default)
        {
            return Task.Run(() => _liteStorage.Delete(path), cancellationToken);
        }

        public async Task<byte[]> DownloadAsync(string path, CancellationToken cancellationToken)
        {
            using (var stream = new MemoryStream())
            {
                await Task.Run(() => _liteStorage.Download(path, stream), cancellationToken);

                return stream.ToArray();
            }
        }

        public Task<bool> ExistsAsync(string path, CancellationToken cancellationToken = default)
        {
            return Task.Run(() => _liteStorage.Exists(path), cancellationToken);
        }

        public async Task UploadAsync(string path, byte[] content, CancellationToken cancellationToken)
        {
            using (var stream = new MemoryStream(content))
            {
                await Task.Run(() => _liteStorage.Upload(path, path, stream), cancellationToken);
            }
        }
    }
}