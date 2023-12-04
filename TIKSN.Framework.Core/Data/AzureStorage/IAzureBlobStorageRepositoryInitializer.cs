namespace TIKSN.Data.AzureStorage;

public interface IAzureBlobStorageRepositoryInitializer
{
    Task InitializeAsync(CancellationToken cancellationToken);
}
