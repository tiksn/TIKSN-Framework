using Azure.Storage.Blobs;
using Microsoft.Extensions.Configuration;

namespace TIKSN.Data.AzureStorage;

public class AzureBlobStorageRepository
    : IAzureBlobStorageRepository
    , IAzureBlobStorageRepositoryInitializer
{
    private readonly BlobContainerClient blobContainerClient;
    private readonly string containerName;

    public AzureBlobStorageRepository(
        string containerName,
        IConfigurationRoot configuration,
        string connectionStringKey)
    {
        if (string.IsNullOrWhiteSpace(containerName))
        {
            throw new ArgumentException($"'{nameof(containerName)}' cannot be null or empty or whitespace.", nameof(containerName));
        }

        ArgumentNullException.ThrowIfNull(configuration);

        this.blobContainerClient = new BlobContainerClient(configuration.GetConnectionString(connectionStringKey), this.containerName);

        this.containerName = containerName;
    }

    public Task DeleteAsync(string path, CancellationToken cancellationToken)
    {
        var blob = this.blobContainerClient.GetBlobClient(path);

        return blob.DeleteAsync(cancellationToken: cancellationToken);
    }

    public async Task<IFile> DownloadAsync(string path, CancellationToken cancellationToken)
    {
        var blob = this.blobContainerClient.GetBlobClient(path);

        var contentResult = await blob.DownloadContentAsync(cancellationToken: cancellationToken).ConfigureAwait(false);

        return new File(blob.Name, contentResult.Value.Content.ToArray());
    }

    public async Task<bool> ExistsAsync(string path, CancellationToken cancellationToken)
    {
        var blob = this.blobContainerClient.GetBlobClient(path);

        var result = await blob.ExistsAsync(cancellationToken).ConfigureAwait(false);

        return result.Value;
    }

    public Task InitializeAsync(CancellationToken cancellationToken)
        => this.blobContainerClient.CreateIfNotExistsAsync(cancellationToken: cancellationToken);

    public Task UploadAsync(string path, byte[] content, CancellationToken cancellationToken)
    {
        var blob = this.blobContainerClient.GetBlobClient(path);

        var contentStream = new MemoryStream(content);

        return blob.UploadAsync(contentStream, cancellationToken);
    }
}
