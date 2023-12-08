using LiteDB;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.FileProviders;

namespace TIKSN.Data.LiteDB;

/// <summary>
///     Create LiteDB database
/// </summary>
public class LiteDbDatabaseProvider : ILiteDbDatabaseProvider
{
    private readonly IConfiguration configuration;
    private readonly string connectionStringKey;
    private readonly IFileProvider fileProvider;

    public LiteDbDatabaseProvider(
        IConfiguration configuration,
        string connectionStringKey,
        IFileProvider fileProvider = null)
    {
        this.configuration = configuration;
        this.connectionStringKey = connectionStringKey;
        this.fileProvider = fileProvider;
    }

    /// <summary>
    ///     Creates LiteDB database with mapper
    /// </summary>
    /// <param name="mapper">Mapper</param>
    /// <returns>Database instance</returns>
    public LiteDatabase GetDatabase(BsonMapper mapper)
    {
        var connectionString =
            new ConnectionString(this.configuration.GetConnectionString(this.connectionStringKey));

        if (this.fileProvider != null)
        {
            connectionString.Filename = this.fileProvider.GetFileInfo(connectionString.Filename).PhysicalPath;
        }

        return new LiteDatabase(connectionString);
    }

    /// <summary>
    ///     Creates LiteDB database
    /// </summary>
    /// <returns>Database instance</returns>
    public LiteDatabase GetDatabase() => this.GetDatabase(mapper: null);
}
