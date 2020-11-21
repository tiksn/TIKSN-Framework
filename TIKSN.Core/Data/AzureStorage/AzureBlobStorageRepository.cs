using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Blob;
using Microsoft.Extensions.Configuration;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace TIKSN.Data.AzureStorage
{
    public class AzureBlobStorageRepository : AzureStorageBase, IAzureBlobStorageRepository, IAzureBlobStorageRepositoryInitializer
    {
        private readonly BlobContainerPublicAccessType _accessType;
        private readonly string _containerName;
        private readonly OperationContext _operationContext;
        private readonly BlobRequestOptions _options;

        public AzureBlobStorageRepository(
            string containerName,
            IConfigurationRoot configuration,
            string connectionStringKey,
            BlobContainerPublicAccessType accessType,
            BlobRequestOptions options,
            OperationContext operationContext) : base(configuration, connectionStringKey)
        {
            _containerName = containerName;
            _accessType = accessType;
            _options = options;
            _operationContext = operationContext;
        }

        public Task DeleteAsync(string path, CancellationToken cancellationToken)
        {
            var blob = GetCloudBlobContainer().GetBlobReference(path);

            return blob.DeleteAsync(DeleteSnapshotsOption.IncludeSnapshots, AccessCondition.GenerateEmptyCondition(), _options, _operationContext, cancellationToken);
        }

        public async Task<IFile> DownloadAsync(string path, CancellationToken cancellationToken)
        {
            var blob = GetCloudBlobContainer().GetBlobReference(path);

            using (var stream = new MemoryStream())
            {
                await blob.DownloadToStreamAsync(stream, AccessCondition.GenerateEmptyCondition(), _options, _operationContext, cancellationToken);

                return new File(blob.Name, stream.ToArray());
            }
        }

        public Task<bool> ExistsAsync(string path, CancellationToken cancellationToken)
        {
            var blob = GetCloudBlobContainer().GetBlobReference(path);

            return blob.ExistsAsync(_options, _operationContext, cancellationToken);
        }

        public Task InitializeAsync(CancellationToken cancellationToken)
        {
            return GetCloudBlobContainer().CreateIfNotExistsAsync(_accessType, _options, _operationContext, cancellationToken);
        }

        public Task UploadAsync(string path, byte[] content, CancellationToken cancellationToken)
        {
            var blob = GetCloudBlobContainer().GetBlockBlobReference(path);

            return blob.UploadFromByteArrayAsync(content, 0, content.Length, AccessCondition.GenerateEmptyCondition(), _options, _operationContext, cancellationToken);
        }

        protected CloudBlobContainer GetCloudBlobContainer()
        {
            var storageAccount = GetCloudStorageAccount();

            var blobClient = storageAccount.CreateCloudBlobClient();

            return blobClient.GetContainerReference(_containerName);
        }
    }
}