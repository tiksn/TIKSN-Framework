using LiteDB;

namespace TIKSN.Data.LiteDB
{
    public interface ILiteDbDatabaseProvider
    {
        LiteDatabase GetDatabase();
    }
}