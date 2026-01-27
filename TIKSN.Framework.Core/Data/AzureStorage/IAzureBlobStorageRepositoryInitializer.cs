namespace TIKSN.Data.AzureStorage;

public interface IAzureBlobStorageRepositoryInitializer
{
    public Task InitializeAsync(CancellationToken cancellationToken);
}
