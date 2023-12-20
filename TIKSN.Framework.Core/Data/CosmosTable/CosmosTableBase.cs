using Microsoft.Azure.Cosmos.Table;
using Microsoft.Extensions.Configuration;

namespace TIKSN.Data.CosmosTable;

public abstract class CosmosTableBase
{
    private readonly IConfigurationRoot _configuration;
    private readonly string _connectionStringKey;

    protected CosmosTableBase(IConfigurationRoot configuration, string connectionStringKey)
    {
        this._configuration = configuration;
        this._connectionStringKey = connectionStringKey;
    }

    protected CloudStorageAccount GetCloudStorageAccount() =>
        CloudStorageAccount.Parse(this._configuration.GetConnectionString(this._connectionStringKey));
}
