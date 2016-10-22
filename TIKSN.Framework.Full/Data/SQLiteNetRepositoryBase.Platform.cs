using SQLite.Net;
using SQLite.Net.Async;
using SQLite.Net.Platform.Win32;

namespace TIKSN.Data
{
    public abstract partial class SQLiteNetRepositoryBase<T>
    {
        protected SQLiteAsyncConnection CreateConnection()
        {
            var connectionString = new SQLiteConnectionString(databaseConfiguration.GetConfiguration().DatabasePath, true);

            var connectionWithLock = new SQLiteConnectionWithLock(new SQLitePlatformWin32(), connectionString);
            return new SQLiteAsyncConnection(() => connectionWithLock);
        }
    }
}