using System.Threading;
using System.Threading.Tasks;

namespace TIKSN.Data
{
	public interface IFileRepository
	{
		Task DeleteAsync(string path, CancellationToken cancellationToken = default);

		Task<byte[]> DownloadAsync(string path, CancellationToken cancellationToken = default);

		Task<bool> ExistsAsync(string path, CancellationToken cancellationToken = default);

		Task UploadAsync(string path, byte[] content, CancellationToken cancellationToken = default);
	}
}
