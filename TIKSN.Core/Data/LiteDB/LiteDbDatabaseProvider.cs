using LiteDB;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.FileProviders;

namespace TIKSN.Data.LiteDB
{
    public class LiteDbDatabaseProvider : ILiteDbDatabaseProvider
    {
        private readonly IConfigurationRoot _configuration;
        private readonly string _connectionStringKey;
        private readonly IFileProvider _fileProvider;

        public LiteDbDatabaseProvider(IConfigurationRoot configuration, string connectionStringKey, IFileProvider fileProvider = null)
        {
            _configuration = configuration;
            _connectionStringKey = connectionStringKey;
            _fileProvider = fileProvider;
        }

        public LiteDatabase GetDatabase()
        {
            var connectionString = new ConnectionString(_configuration.GetConnectionString(_connectionStringKey));

            if (_fileProvider != null)
                connectionString.Filename = _fileProvider.GetFileInfo(connectionString.Filename).PhysicalPath;

            return new LiteDatabase(connectionString);
        }
    }
}