using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Blob;
using Microsoft.Extensions.Configuration;

namespace TIKSN.Data.AzureStorage;

public class AzureBlobStorageRepository : AzureStorageBase, IAzureBlobStorageRepository,
    IAzureBlobStorageRepositoryInitializer
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
        this._containerName = containerName;
        this._accessType = accessType;
        this._options = options;
        this._operationContext = operationContext;
    }

    public Task DeleteAsync(string path, CancellationToken cancellationToken)
    {
        var blob = this.GetCloudBlobContainer().GetBlobReference(path);

        return blob.DeleteAsync(DeleteSnapshotsOption.IncludeSnapshots, AccessCondition.GenerateEmptyCondition(),
            this._options, this._operationContext, cancellationToken);
    }

    public async Task<IFile> DownloadAsync(string path, CancellationToken cancellationToken)
    {
        var blob = this.GetCloudBlobContainer().GetBlobReference(path);

        using var stream = new MemoryStream();
        await blob.DownloadToStreamAsync(stream, AccessCondition.GenerateEmptyCondition(), this._options,
            this._operationContext, cancellationToken).ConfigureAwait(false);

        return new File(blob.Name, stream.ToArray());
    }

    public Task<bool> ExistsAsync(string path, CancellationToken cancellationToken)
    {
        var blob = this.GetCloudBlobContainer().GetBlobReference(path);

        return blob.ExistsAsync(this._options, this._operationContext, cancellationToken);
    }

    public Task UploadAsync(string path, byte[] content, CancellationToken cancellationToken)
    {
        var blob = this.GetCloudBlobContainer().GetBlockBlobReference(path);

        return blob.UploadFromByteArrayAsync(content, 0, content.Length, AccessCondition.GenerateEmptyCondition(),
            this._options, this._operationContext, cancellationToken);
    }

    public Task InitializeAsync(CancellationToken cancellationToken) => this.GetCloudBlobContainer()
        .CreateIfNotExistsAsync(this._accessType, this._options, this._operationContext, cancellationToken);

    protected CloudBlobContainer GetCloudBlobContainer()
    {
        var storageAccount = this.GetCloudStorageAccount();

        var blobClient = storageAccount.CreateCloudBlobClient();

        return blobClient.GetContainerReference(this._containerName);
    }
}
