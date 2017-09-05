using LiteDB;
using Microsoft.Extensions.Configuration;

namespace TIKSN.Data.LiteDB
{
	public class LiteDbDatabaseProvider : ILiteDbDatabaseProvider
	{
		private readonly IConfigurationRoot _configuration;
		private readonly string _connectionStringKey;

		public LiteDbDatabaseProvider(IConfigurationRoot configuration, string connectionStringKey)
		{
			_configuration = configuration;
			_connectionStringKey = connectionStringKey;
		}

		public LiteDatabase GetDatabase()
		{
			var connectionString = _configuration.GetConnectionString(_connectionStringKey);

			return new LiteDatabase(connectionString);
		}
	}
}
