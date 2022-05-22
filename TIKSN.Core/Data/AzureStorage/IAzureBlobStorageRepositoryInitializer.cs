using System.Threading;
using System.Threading.Tasks;

namespace TIKSN.Data.AzureStorage
{
    public interface IAzureBlobStorageRepositoryInitializer
    {
        Task InitializeAsync(CancellationToken cancellationToken);
    }
}
