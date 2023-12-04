using Microsoft.Azure.Storage;
using Microsoft.Extensions.Configuration;

namespace TIKSN.Data.AzureStorage;

public abstract class AzureStorageBase
{
    private readonly IConfigurationRoot _configuration;
    private readonly string _connectionStringKey;

    protected AzureStorageBase(IConfigurationRoot configuration, string connectionStringKey)
    {
        this._configuration = configuration;
        this._connectionStringKey = connectionStringKey;
    }

    protected CloudStorageAccount GetCloudStorageAccount() =>
        CloudStorageAccount.Parse(this._configuration.GetConnectionString(this._connectionStringKey));
}
