using Microsoft.Extensions.Configuration;
using Microsoft.WindowsAzure.Storage;

namespace TIKSN.Data.AzureStorage
{
	public abstract class AzureStorageBase
	{
		private readonly IConfigurationRoot _configuration;
		private readonly string _connectionStringKey;

		protected AzureStorageBase(IConfigurationRoot configuration, string connectionStringKey)
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