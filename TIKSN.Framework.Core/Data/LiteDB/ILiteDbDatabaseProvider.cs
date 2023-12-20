using LiteDB;

namespace TIKSN.Data.LiteDB;

/// <summary>
///     Create LiteDB database
/// </summary>
public interface ILiteDbDatabaseProvider
{
    /// <summary>
    ///     Creates LiteDB database
    /// </summary>
    /// <returns>Database instance</returns>
    LiteDatabase GetDatabase();

    /// <summary>
    ///     Creates LiteDB database with mapper
    /// </summary>
    /// <param name="mapper">Mapper</param>
    /// <returns>Database instance</returns>
    LiteDatabase GetDatabase(BsonMapper mapper);
}
