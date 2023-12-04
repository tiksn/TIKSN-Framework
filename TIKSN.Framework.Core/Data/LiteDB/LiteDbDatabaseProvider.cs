using LiteDB;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.FileProviders;

namespace TIKSN.Data.LiteDB;

/// <summary>
///     Create LiteDB database
/// </summary>
public class LiteDbDatabaseProvider : ILiteDbDatabaseProvider
{
    private readonly IConfiguration _configuration;
    private readonly string _connectionStringKey;
    private readonly IFileProvider _fileProvider;

    public LiteDbDatabaseProvider(IConfiguration configuration, string connectionStringKey,
        IFileProvider fileProvider = null)
    {
        this._configuration = configuration;
        this._connectionStringKey = connectionStringKey;
        this._fileProvider = fileProvider;
    }

    /// <summary>
    ///     Creates LiteDB database with mapper
    /// </summary>
    /// <param name="mapper">Mapper</param>
    /// <returns></returns>
    public LiteDatabase GetDatabase(BsonMapper mapper)
    {
        var connectionString =
            new ConnectionString(this._configuration.GetConnectionString(this._connectionStringKey));

        if (this._fileProvider != null)
        {
            connectionString.Filename = this._fileProvider.GetFileInfo(connectionString.Filename).PhysicalPath;
        }

        return new LiteDatabase(connectionString);
    }

    /// <summary>
    ///     Creates LiteDB database
    /// </summary>
    /// <returns></returns>
    public LiteDatabase GetDatabase() => this.GetDatabase(null);
}
