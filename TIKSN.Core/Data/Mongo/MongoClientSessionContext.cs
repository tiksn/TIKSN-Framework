using System;
using LanguageExt;
using MongoDB.Driver;

namespace TIKSN.Data.Mongo
{
    public class MongoClientSessionContext : IMongoClientSessionStore, IMongoClientSessionProvider
    {
        private Option<IClientSessionHandle> _clientSessionHandle = Option<IClientSessionHandle>.None;

        public Option<IClientSessionHandle> GetClientSessionHandle() => this._clientSessionHandle;

        public void SetClientSessionHandle(IClientSessionHandle clientSessionHandle)
        {
            if (clientSessionHandle == null)
            {
                throw new ArgumentNullException(nameof(clientSessionHandle));
            }

            this._clientSessionHandle = Option<IClientSessionHandle>.Some(clientSessionHandle);
        }

        public void ClearClientSessionHandle() => this._clientSessionHandle = Option<IClientSessionHandle>.None;
    }
}
