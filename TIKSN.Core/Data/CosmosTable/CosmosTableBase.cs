using Microsoft.Azure.Cosmos.Table;
using Microsoft.Extensions.Configuration;

namespace TIKSN.Data.CosmosTable
{
    public abstract class CosmosTableBase
    {
        private readonly IConfigurationRoot _configuration;
        private readonly string _connectionStringKey;

        protected CosmosTableBase(IConfigurationRoot configuration, string connectionStringKey)
        {
            _configuration = configuration;
            _connectionStringKey = connectionStringKey;
        }

        protected CloudStorageAccount GetCloudStorageAccount()
        {
            return CloudStorageAccount.Parse(_configuration.GetConnectionString(_connectionStringKey));
        }
    }
}