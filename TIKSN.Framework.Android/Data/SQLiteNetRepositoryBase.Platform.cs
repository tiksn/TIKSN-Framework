using SQLite.Net;
using SQLite.Net.Async;
using SQLite.Net.Platform.XamarinAndroid;

namespace TIKSN.Data
{
    public abstract partial class SQLiteNetRepositoryBase<T>
    {
        protected SQLiteAsyncConnection CreateConnection()
        {
            //var connectionString = new SQLiteConnectionString(Path.Combine(ApplicationData.Current.LocalFolder.Path, databaseConfiguration.GetConfiguration().DatabasePath), true);
            var connectionString = new SQLiteConnectionString(databaseConfiguration.GetConfiguration().DatabasePath, true);

            var connectionWithLock = new SQLiteConnectionWithLock(new SQLitePlatformAndroid(), connectionString);
            return new SQLiteAsyncConnection(() => connectionWithLock);
        }
    }
}
