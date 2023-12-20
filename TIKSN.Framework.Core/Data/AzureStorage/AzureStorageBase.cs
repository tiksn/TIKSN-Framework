using Microsoft.Azure.Storage;
using Microsoft.Extensions.Configuration;

namespace TIKSN.Data.AzureStorage;

public abstract class AzureStorageBase
{
    private readonly IConfigurationRoot configuration;
    private readonly string connectionStringKey;

    protected AzureStorageBase(IConfigurationRoot configuration, string connectionStringKey)
    {
        this.configuration = configuration;
        this.connectionStringKey = connectionStringKey;
    }

    protected CloudStorageAccount GetCloudStorageAccount() =>
        CloudStorageAccount.Parse(this.configuration.GetConnectionString(this.connectionStringKey));
}
