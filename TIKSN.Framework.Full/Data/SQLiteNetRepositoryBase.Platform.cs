using SQLite.Net.Async;
using System;

namespace TIKSN.Data
{
    public abstract partial class SQLiteNetRepositoryBase<T>
    {
        protected SQLiteAsyncConnection CreateConnection()
        {
            throw new NotImplementedException();
        }
    }
}
