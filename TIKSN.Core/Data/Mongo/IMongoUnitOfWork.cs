using System;

namespace TIKSN.Data.Mongo
{
    public interface IMongoUnitOfWork : IUnitOfWork
    {
        public IServiceProvider Services { get; }
    }
}
