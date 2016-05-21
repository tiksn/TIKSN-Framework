using SQLite.Net;
using SQLite.Net.Async;
using SQLite.Net.Platform.WinRT;
using System.IO;
using Windows.Storage;

namespace TIKSN.Data
{
    public abstract partial class SQLiteNetRepositoryBase<T>
    {
        protected SQLiteAsyncConnection CreateConnection()
        {
            var connectionString = new SQLiteConnectionString(Path.Combine(ApplicationData.Current.LocalFolder.Path, databaseConfiguration.GetConfiguration().DatabasePath), true);

            var connectionWithLock = new SQLiteConnectionWithLock(new SQLitePlatformWinRT(), connectionString);
            var connection = new SQLiteAsyncConnection(() => connectionWithLock);

            return connection;
        }
    }
}
